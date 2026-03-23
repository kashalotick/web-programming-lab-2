<script setup lang="ts">

import ReservationTable from "~/components/table/ReservationTable.vue";
import {ReservationAPI} from "~/services/reservation-service";
import type {ReservationsQueryParams} from "~/types/common";
import type {Reservation, ReservationDtoGet} from "~/types/reservation";
import type {RoomDtoGet} from "~/types/room";
import {RoomAPI} from "~/services/room-service";


const reservations = ref<Reservation[]>([])
const rooms = ref<RoomDtoGet[]>([])
const roomNames = computed(() => {
  const map: Record<number, string> = {}
  rooms.value.forEach(r => map[r.id] = r.name ?? '')
  return map
})

fetchRooms()
fetchReservations()
async function fetchRooms(params: ReservationsQueryParams = {}) {
  try {
    const result = await RoomAPI.getAll() as RoomDtoGet[]
    rooms.value = result
  } catch (e) {
    console.error("Failed to fetch rooms", e)
    // @ts-ignore
    console.error(e.data)
  }
}

async function fetchReservations(params: ReservationsQueryParams = {}) {
  console.log(params)
  try {
    const result = await ReservationAPI.getAll(params) as ReservationDtoGet[]
    reservations.value = result.map(r => ({
      ...r,
      checkIn: new Date(r.checkIn),
      checkOut: new Date(r.checkOut),
      createdAt: new Date(r.createdAt),
    }))
  } catch (e) {
    console.error("Failed to fetch reservations", e)
    // @ts-ignore
    console.error(e.data)
  }
}

async function viewReservation(id: number) {
  console.log(`View reservation ${id}`)
}
async function cancelReservation(id: number) {
  console.log(`Cancel reservation ${id}`)

}
async function deleteReservation(id: number) {
  console.log(`Delete reservation ${id}`)
}

</script>

<template>
  <main>
    <PageHeader heading="Бронювання">
      <template #buttons>
        <NuxtLink to="/admin/reservations/new" class="no-text-decoration">
        <button class="style-1">
          <Icon name="lucide:plus" size="1.5rem" color="white"/>
          Нове бронювання
        </button>
        </NuxtLink>
      </template>
    </PageHeader>
    <ReservationTable
        :room-names="roomNames"
        :reservations="reservations"
        @view="viewReservation"
        @cancel="cancelReservation"
        @delete="deleteReservation"
        @changeFilter="fetchReservations"
    >
    </ReservationTable>
  </main>

</template>

<style scoped>
main {
  background: var(--background);

  flex: 1;
  padding: 2rem 2rem;

}
</style>