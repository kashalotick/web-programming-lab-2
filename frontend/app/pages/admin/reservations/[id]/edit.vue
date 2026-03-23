<script setup lang="ts">
import {ReservationAPI} from "~/services/reservation-service";
import type {ReservationFullDtoGet} from "~/types/reservation";
import ReservationPage from "~/components/reservations/ReservationPage.vue";

const route = useRoute()

const {data} = await useAsyncData(
    `reservation-${route.params.id}`,
    async () => {
      const result = await ReservationAPI.getFull(Number(route.params.id)) as ReservationFullDtoGet
      return result
    }
)
if (!data.value) {
  throw createError({status: 404, fatal: true})
}
</script>

<template>
  <ReservationPage :id="route.params.id" :reservation="data!" mode="edit"/>

</template>

<style scoped>

</style>