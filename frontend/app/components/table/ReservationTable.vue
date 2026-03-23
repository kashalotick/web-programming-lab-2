<script setup lang="ts">
import type {Reservation} from "~/types/reservation";

const props = defineProps<{
  roomNames: string[]
  reservations: Reservation[]
}>()


type filters = {
  status: 'all' | 'active' | 'cancelled'
  rooms: number | 'all',
  range: [Date, Date]
}
defineEmits<{
  sort: ['grand_total' | 'is_active' | 'created_at', boolean],
}>()
</script>

<template>

</template>

<style scoped>

</style>