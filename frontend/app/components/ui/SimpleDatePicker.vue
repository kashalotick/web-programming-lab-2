<!-- AdminDateRangePicker.vue -->
<script setup lang="ts">
import CalendarDropdown from '~/components/ui/CalendarDropdown.vue'
import InteractiveCalendar from '~/components/ui/InteractiveCalendar.vue'

const props = defineProps<{
  fromTarget: HTMLElement | null
  toTarget: HTMLElement | null
  activeField: 'from' | 'to' | null
  from: string | null
  to: string | null
}>()

const emit = defineEmits<{
  'update:activeField': ['from' | 'to' | null]
  'update:from': [string | null]
  'update:to': [string | null]
}>()

const emptyDates = {}

const currentTarget = computed(() =>
    props.activeField === 'from' ? props.fromTarget : props.toTarget
)

const coMin = computed<Date | null>(() => {
  if (!props.from) return null
  const d = new Date(props.from)
  d.setHours(0, 0, 0, 0)
  return d
})

// скидаємо to якщо from змінився і to раніше
watch(() => props.from, (val) => {
  if (val && props.to && new Date(props.to) <= new Date(val)) {
    emit('update:to', null)
  }
})

const calendarProps = computed(() => {
  if (props.activeField === 'from') {
    return {
      modelValue: props.from,
      'onUpdate:modelValue': (val: string | null) => {
        emit('update:from', val)
        emit('update:activeField', 'to')
      },
      dateMin: null,
      dateMax: null,
      allowDisabled: true,
    }
  } else {
    return {
      modelValue: props.to,
      'onUpdate:modelValue': (val: string | null) => {
        emit('update:to', val)
        emit('update:activeField', null)
      },
      dateMin: coMin.value,
      dateMax: null,
      allowDisabled: true,
    }
  }
})

function handleOk() {
  emit('update:activeField', null)
}

function handleReset() {
  if (props.activeField === 'from') {
    emit('update:from', null)
    emit('update:to', null)
  } else {
    emit('update:to', null)
  }
  emit('update:activeField', null)
}



</script>

<template>
  <CalendarDropdown
      :anchor-target="currentTarget"
      :is-open="!!activeField"
      @close="emit('update:activeField', null)"
  >
    <InteractiveCalendar
        ref="calendar"
        v-bind="calendarProps"
        :dates="emptyDates"
        @ok="handleOk"
        @reset="handleReset"
    />
  </CalendarDropdown>
</template>