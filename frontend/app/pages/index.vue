<script setup lang="ts">

import {type ReservationDtoGet, restore} from "~/types/reservation";
import DateRangePicker from "~/components/DateRangePicker.vue";

const config = useRuntimeConfig()

// async function click() {
//   let result = await $fetch('rooms', {
//     baseURL: config.public.apiBase,
//   })
//   console.log('/rooms')
//   console.log(result)
//
//   console.log('/reservations')
//   result = await $fetch('reservations', {
//     baseURL: config.public.apiBase,
//   })
//   console.log(result)
// }
// async function fetchReservations() {
//   const result = await $fetch('reservations', {
//     baseURL: config.public.apiBase
//   }) as ReservationDtoGet[];
//   const res = Object.fromEntries(result.map(r => [r.id, restore(r)]))
//   console.log(res)
// }
const MAX_STAY = 7
const dates: Record<string, {
  price: number;
  available: boolean;
}> = {
  '2026-03-16': {price: 1200, available: true},
  '2026-03-17': {price: 1200, available: true},
  '2026-03-18': {price: 1200, available: true},
  '2026-03-19': {price: 1200, available: true},
  '2026-03-20': {price: 1200, available: true},

  '2026-03-21': {price: 1200, available: true},
  '2026-03-22': {price: 1200, available: true},
  '2026-03-23': {price: 1200, available: true},
  '2026-03-24': {price: 1200, available: true},
  '2026-03-25': {price: 1200, available: true},
  '2026-03-26': {price: 1200, available: true},
  '2026-03-27': {price: 1200, available: true},
  '2026-03-28': {price: 1200, available: true},
  '2026-03-29': {price: 1200, available: true},
  '2026-03-30': {price: 1200, available: true},
  '2026-03-31': {price: 1200, available: true},

  '2026-04-01': {price: 1200, available: true},
  '2026-04-02': {price: 1200, available: true},
  '2026-04-03': {price: 1200, available: true},

  // дата відсутня → заблокована автоматично
}
const activeField = ref<'checkIn' | 'checkOut' | null>(null)
const checkIn = ref<string | null>(null)
const checkOut = ref<string | null>(null)
const inRef = useTemplateRef<HTMLElement>('checkIn')
const outRef = useTemplateRef<HTMLElement>('checkOut')
const inInputRef = useTemplateRef<HTMLElement>('checkInInput')
const outInputRef = useTemplateRef<HTMLElement>('checkOutInput')

watch(activeField, (newVal) => {
  if (newVal === 'checkIn') {
    inInputRef.value?.focus()
  }
  if (newVal === 'checkOut') {
    outInputRef.value?.focus()
  }
})

const format = (date: string | null) => {
  if (!date) return ''
  const dateObj = new Date(date)
  return dateObj.toLocaleDateString('uk-UA')
}

</script>

<template>
  <div class="booking-inputs">
    <div class="input-wrapper" id="checkIn" ref="checkIn">
      <input ref="checkInInput" @focus="activeField = 'checkIn'" :value="format(checkIn)" readonly/>
    </div>
    <div class="input-wrapper" id="checkOut" ref="checkOut">
      <input ref="checkOutInput" @focus="activeField = 'checkOut'" :value="format(checkOut)" readonly/>
    </div>
  </div>

  <DateRangePicker
      v-model:activeField="activeField"
      v-model:checkIn="checkIn"
      v-model:checkOut="checkOut"
      :check-in-target="inRef"
      :check-out-target="outRef"
      :dates="dates"
      :max-stay="MAX_STAY"
  />
</template>

<style scoped>
.booking-inputs {
  display: flex;
  gap: 16px;
}
.input-wrapper {
  position: relative;
}

</style>