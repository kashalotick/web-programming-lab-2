<script setup lang="ts">
import type {ReservationDtoGet, ReservationFullDtoGet} from "~/types/reservation";
import {ReservationAPI} from "~/services/reservation-service";
const router = useRouter()

const props = defineProps<{
  reservation: ReservationFullDtoGet
  mode: 'view' | 'edit' | 'create'
}>()

const heading = computed(() => {
  switch (props.mode) {
    case 'view':
      return 'Бронювання #' + props.reservation.id
    case 'edit':
      return 'Бронювання #' + props.reservation.id
    case 'create':
      return 'Нове бронювання'
    default:
      return ''
  }
})


async function fetchReservation(id: number) {
  try {
    const result = await ReservationAPI.getFull(id) as ReservationFullDtoGet
    console.log(result)
  } catch (e) {
    console.error(`Failed to fetch reservation ${id}`, e)
    // @ts-ignore
    console.error(e.data)
  }
}



async function toList() {
  await navigateTo(`/admin/reservations`)
}
async function edit(id: number) {
  await navigateTo(`/admin/reservations/${id}/edit`)
}
async function cancelEdit(id: number) {
  goBack()
}
async function delete_(id: number) {
  // TODO: ok popup
  await navigateTo(`/admin/reservations`)
}
async function save(id: number) {
  await navigateTo(`/admin/reservations/${id}`)
}

async function create(id: number) {
  await navigateTo(`/admin/reservations/${id}`)
}

function goBack() {
  if (window.history.length > 1) {
    router.back()
  } else {
    router.push('/admin/reservations')
  }
}
</script>

<template>
  <main>
    <PageHeader
        :heading
        :mode
    >
      <template #buttons>
        <button @click="toList" v-if="mode == 'view'" class="btn btn-ghost">
          До списку
        </button>

        <button @click="edit(reservation.id)" v-if="mode == 'view'" class="btn btn-secondary">
          Редагувати
        </button>
        <button @click="delete_(reservation.id)" v-if="mode == 'view'" class="btn btn-danger">
          Видалити
        </button>
        <button @click="save(reservation.id)" v-if="mode == 'edit'" class="btn btn-primary">
          Зберегти
        </button>
        <button @click="cancelEdit(reservation.id)" v-if="mode == 'edit'" class="btn btn-secondary">
          Скасувати
        </button>


        <button @click="create(reservation.id)" v-if="mode == 'create'" class="btn btn-primary">
          Зберегти
        </button>


      </template>
      <template #secondary>
        <div class="page-meta">
          Створено: {{reservation.createdAt}}
        </div>
      </template>
    </PageHeader>

  </main>


</template>

<style scoped>
main {
  background: var(--background);

  flex: 1;
  padding: 2rem 2rem;
  height: 100dvh;

}
</style>