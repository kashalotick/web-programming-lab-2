<script setup lang="ts">
import {type Reservation, type ReservationDtoGet, restore} from "~/types/reservation";

const config = useRuntimeConfig()


const reservations = ref<Record<number, Reservation>>({})
fetchReservations()


async function fetchReservations() {
  const result = await $fetch('reservations', {
    baseURL: config.public.apiBase
  }) as ReservationDtoGet[];
  reservations.value = Object.fromEntries(result.map(r => [r.id, restore(r)]))
}

function editReservation(id: number) {
  console.log(`Edit reservation with id ${id}`) // TODO: make api
}

function cancelReservation(id: number) {
  console.log(`Cancel reservation with id ${id}`) // TODO: make api
  const reservation = reservations.value[id]
  if (reservation) {
    reservation.isActive = false
  }
}

function deleteReservation(id: number) {
  console.log(`Delete reservation with id ${id}`) // TODO: make api
  delete reservations.value[id]
}


</script>

<template>
  <div class="reservation-list">
    <ReservationTile
        v-for="reservation in reservations"
        :reservation="reservation"
        :key="reservation.id"
        @edit="editReservation"
        @cancel="cancelReservation"
        @delete="deleteReservation"
    />
  </div>
</template>

<style scoped>
.reservation-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>