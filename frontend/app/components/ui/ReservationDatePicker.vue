<script setup lang="ts">
import type {AvailabilityDtoGet} from "~/types/room";
import CalendarDropdown from "~/components/ui/CalendarDropdown.vue";
import InteractiveCalendar from "~/components/ui/InteractiveCalendar.vue";

const {checkInTarget, checkOutTarget, activeField, checkIn, checkOut, dates, maxStay = 7} = defineProps<{

  checkInTarget: HTMLElement | null,
  checkOutTarget: HTMLElement | null,
  activeField: 'checkIn' | 'checkOut' | null,
  checkIn: string | null,
  checkOut: string | null,
  dates: AvailabilityDtoGet,
  maxStay?: number,
}>()
const emit = defineEmits(['update:activeField', 'update:modelValue', 'update:checkIn', 'update:checkOut'])

const currentTarget = computed(() => {
  if (activeField === 'checkIn') return checkInTarget
  if (activeField === 'checkOut') return checkOutTarget
  return null
})


const ciMin = ref<Date>((() => {
  const d = new Date()
  d.setHours(0, 0, 0, 0)
  return d
})())

const coMin = computed<Date>(() => {
  if (!checkIn) {
    return ciMin.value
  }
  const d = new Date(checkIn)
  d.setDate(d.getDate())
  return d
})
const coMax = computed(() => {
  if (!checkIn) {
    const d = new Date(ciMin.value)
    d.setDate(d.getDate() - 1)
    return d
  }
  const d = new Date(checkIn)
  for (let i = 0; i < maxStay; i++) {
    d.setDate(d.getDate() + 1)

    const dateStr = d.toISOString().split('T')[0]
    const day = dates[dateStr!]

    if (!day || !day.isAvailable) {
      return d
    }
  }

  return d
})

watch(() => checkIn, (newCi, old) => { // TODO: works unproperly
  if (newCi && checkOut) {
    const checkOutDate = new Date(checkOut)
    const ciDate = new Date(newCi)
    if (checkOutDate <= ciDate) {
      emit('update:checkOut', null)
    }
    if (checkOut && coMax.value && checkOutDate > coMax.value) {
      emit('update:checkOut', null)

    }
  }
})

const calendarProps = computed(() => {
  if (activeField === 'checkIn') {
    return {
      modelValue: checkIn,
      'onUpdate:modelValue': (val: string | null) => {
        emit('update:checkIn', val)
        emit('update:activeField', 'checkOut')
      },
      dateMin: ciMin.value,
      dateMax: null,
      allowDisabled: false
    }
  } else {
    return {
      modelValue: checkOut,
      'onUpdate:modelValue': (val: string | null) => {
        emit('update:checkOut', val)
        emit('update:activeField', null)
      },
      dateMin: coMin.value,
      dateMax: coMax.value,
      allowDisabled: true
    }
  }
})

function handleOk() {
  close()
}

function handleReset() {
  if (activeField === 'checkIn') {
    emit('update:checkIn', null)
    emit('update:checkOut', null)
  } else {
    emit('update:checkOut', null)
  }
  // emit('update:activeField', null)
}

// const calendarRef = useTemplateRef('calendar')
// const handleClickOutside = (event: MouseEvent) => {
//   const checkInEl = checkInTarget;
//   const checkOutEl = checkOutTarget;
//
//   const calendarEl = calendarRef.value?.$el || calendarRef.value;
//
//   const clickedInsideInput =
//       checkInEl?.contains(event.target) ||
//       checkOutEl?.contains(event.target);
//
//   const clickedInsideCalendar = calendarEl?.contains?.(event.target);
//
//   if (!clickedInsideInput && !clickedInsideCalendar) {
//     emit('update:activeField', null)
//   }
// }

function close() {
  emit('update:activeField', null)
}

// function onEscape(event: KeyboardEvent) {
//   if (event.key === 'Escape') {
//     close()
//   }
// }
//
// const calendarStyle = ref({top: '0px', left: '0px'});
// watch(() => activeField, (val) => {
//   if (val) {
//     nextTick(updatePosition);
//   }
// });
//
// function updatePosition() {
//   const target = currentTarget.value;
//   if (target) {
//     const rect = target.getBoundingClientRect();
//     const top = rect.bottom + window.scrollY;
//     let left = rect.left + window.scrollX;
//
//     const bodyRect = document.body.getBoundingClientRect();
//     if (left + 420 > bodyRect.width) {
//       left = bodyRect.width - 420 - 10;
//     }
//
//     calendarStyle.value = {
//       top: `${top}px`,
//       left: `${left}px`,
//     };
//   }
// }

// onMounted(() => {
//   document.addEventListener('click', handleClickOutside)
//   document.addEventListener('keydown', onEscape)
// })
// onUnmounted(() => {
//   document.removeEventListener('click', handleClickOutside)
//   document.removeEventListener('keydown', onEscape)
// })

</script>

<template>
  <CalendarDropdown
      :anchor-target="currentTarget"
      :is-open="!!activeField"
      @close="close"
  >
    <InteractiveCalendar
        ref="calendar"
        v-bind="calendarProps"
        :dates="dates"
        @ok="handleOk"
        @reset="handleReset"
    />
  </CalendarDropdown>


</template>

<style scoped>



</style>