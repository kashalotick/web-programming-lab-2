<script setup lang="ts">
import type {Reservation} from "~/types/reservation";

const props = defineProps<{
  roomNames: string[]
  reservations: Reservation[]
}>()


type SortBy = 'id' | 'guest' | 'room' | 'checkIn' | 'checkOut' | 'grandTotal' | 'status' | 'createdAt'
type SortWay = 'desc' | 'asc'
type filters = {
  status: 'all' | 'active' | 'cancelled'
  rooms: number | 'all',
  range: [Date, Date]
}
defineEmits<{
  sort: [string, 'asc' | 'desc'],
}>()
</script>

<template>

</template>

<style scoped>

</style>