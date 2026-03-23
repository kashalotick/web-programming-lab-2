<script setup lang="ts">
import type {Reservation} from "~/types/reservation";
import type {ReservationsQueryParams} from "~/types/common";
import Rooms from "~/pages/admin/rooms.vue";

const props = defineProps<{
  roomNames: Record<number, string>
  reservations: Reservation[]
}>()

const emit = defineEmits<{
  view: [number],
  cancel: [number],
  delete: [number],
  changeFilter: [ReservationsQueryParams]
}>()

const fromDate = ref<Date>(new Date())
const toDate = ref<Date>((() => {
  const d = new Date()
  d.setDate(d.getDate() + 60)
  return d
})())

const filter = ref<Required<ReservationsQueryParams>>({
  from: fromDate.value.toISOString().split('T')[0]!,
  to: toDate.value.toISOString().split('T')[0]!,
  roomId: null,
  isActive: null,
  sortBy: null,
  isDescending: false,
})


watch(filter, (newFilter) => {
  emit('changeFilter', newFilter)
}, {deep: true})

</script>

<template>
  <TableFilters
      :room-names
      v-model:from="filter.from"
      v-model:to="filter.to"
      v-model:room-id="filter.roomId"
      v-model:is-active="filter.isActive"
  >
    Знайдено записів: {{ reservations.length }}
  </TableFilters>
  <div class="table-wrap">

    <table>
      <TableHeader
          v-model:is-descending="filter.isDescending"
          v-model:sort-by="filter.sortBy"
      />
      <tbody>
      <TableRow
          v-for="reservation in reservations"
          :reservation="reservation"
          :room-names="roomNames"
          :key="reservation.id"
          @view="$emit('view', $event)"
          @cancel="$emit('cancel', $event)"
          @delete="$emit('delete', $event)"
      />
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-wrap {
  background: var(--white);
  border: 1.5px solid var(--accent-light);
  border-radius: 1rem;
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: .82rem;
}

</style>