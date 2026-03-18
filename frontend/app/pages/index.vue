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

</script>

<template>
  <NuxtLink to="/admin/reservations/new">Гість</NuxtLink>
  <NuxtLink to="/admin/reservations/new">Адмін</NuxtLink>

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