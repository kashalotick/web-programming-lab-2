<script setup lang="ts">
import ReservationDatePicker from "~/components/ui/ReservationDatePicker.vue";
import InteractiveCalendar from "~/components/ui/InteractiveCalendar.vue";
import CalendarDropdown from "~/components/ui/CalendarDropdown.vue";
import SimpleDatePicker from "~/components/ui/SimpleDatePicker.vue";

const props = defineProps<{
  roomNames: Record<number, string>
  from: string | null,
  to: string | null,
  roomId: number | null,
  isActive: boolean | null,
}>()

const emit = defineEmits<{
  'update:from': [string | null],
  'update:to': [string | null],
  'update:room-id': [number | null],
  'update:is-active': [boolean | null],
}>()
const defaultFrom = props.from
const defaultTo = props.to

const from = computed({
  get: () => props.from,
  set: (val) => emit('update:from', val)
})
const to = computed({
  get: () => props.to,
  set: (val) => emit('update:to', val)
})
const roomId = computed({
  get: () => props.roomId,
  set: (val) => emit('update:room-id', val == null ? null : Number(val))
})

const isActive = computed({
  get: () => props.isActive,
  set: (val) => emit('update:is-active', val)
})


const activeField = ref<'from' | 'to' | null>(null)
const inRef = useTemplateRef<HTMLElement>('checkIn')
const outRef = useTemplateRef<HTMLElement>('checkOut')
const inInputRef = useTemplateRef<HTMLElement>('checkInInput')
const outInputRef = useTemplateRef<HTMLElement>('checkOutInput')

watch(activeField, (newVal) => {
  if (newVal === 'from') {
    inInputRef.value?.focus()
  }
  if (newVal === 'to') {
    outInputRef.value?.focus()
  }
  if (!newVal) {
    inInputRef.value?.blur()
    outInputRef.value?.blur()
  }
})

function format(date: string | null) {
  if (!date) return ''
  const dateObj = new Date(date)
  return dateObj.toLocaleDateString('uk-UA')
}
watch(roomId, (newRoomId) => {
  console.log('Room ID changed:', newRoomId)
})
</script>

<template>
  <div class="filters">
    <div class="subinfo">
      <slot></slot>
    </div>
    <div class="sb-field">


      <select v-model="roomId">
        <option :value="null">Всі кімнати</option>
        <option v-for="(name, id) in roomNames" :key="id" :value="id">
          {{ name }}
        </option>
      </select>
    </div>
    <div class="sb-field">

      <select v-model="isActive">

        <option :value="null">Всі статуси</option>
        <option :value="true">Активні</option>
        <option :value="false">Скасовані</option>
      </select>
    </div>
    <div class="sb-field">
      <div class="calendar-input-wrapper" id="checkIn" ref="checkIn">
        <input ref="checkInInput" @focus="activeField = 'from'" :value="format(from)" readonly/>
      </div>
    </div>
    <span class="date-sep"> → </span>
    <div class="sb-field">
      <div class="calendar-input-wrapper" id="checkOut" ref="checkOut">
        <input ref="checkOutInput" @focus="activeField = 'to'" :value="format(to)" readonly/>
      </div>
    </div>
  </div>

  <SimpleDatePicker
      :from-target="inInputRef"
      :to-target="outInputRef"
      :active-field="activeField"
      :from
      :to
      @update:active-field="activeField = $event"
      @update:from="from = $event ?? defaultFrom"
      @update:to="to = $event ?? defaultTo"
  />
</template>

<style scoped>

.filters {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
  flex-wrap: wrap;
  align-items: center;
}
.sb-field {
  width: 200px;
}
.subinfo {
  flex: 1;
  font-family: var(--font-description), sans-serif;
  font-size: 1rem;
  color: #999;
}
.date-sep {
  color: var(--accent);
  font-size: .75rem;
}

</style>