<script setup lang="ts">
import type {ReservationDtoGet, ReservationFullDtoGet} from "~/types/reservation"
import type { ReservationDtoCreate, ReservationDtoUpdate } from "~/types/reservation"
import type { GuestDtoCreate } from "~/types/guest"
import { ReservationAPI } from "~/services/reservation-service"
import ReservationMainCard from "~/components/reservations/ReservationMainCard.vue"
import ReservationRoomCard from "~/components/reservations/ReservationRoomCard.vue"
import ReservationGuestCard from "~/components/reservations/ReservationGuestCard.vue"
import { RoomAPI } from "~/services/room-service"
import type { RoomDtoGet } from "~/types/room"
import PopUp from "~/components/ui/PopUp.vue";

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
}>()

const mode = ref<'view' | 'edit' | 'create'>(props.reservation ? 'view' : 'create')
const rooms = ref<RoomDtoGet[]>([])

// -- Pop ups --------------------------------------------------------
const errorMessage = ref<string>('')
const errorPopUpVisible = ref(false)
const successMessage = ref<string>('')
const successPopUpVisible = ref(false)

// ── DTO для edit ────────────────────────────────────────────────────────────
const reservationDtoUpdate = ref<ReservationDtoUpdate>({
  roomId:     props.reservation?.room?.id     ?? null,
  guestCount: props.reservation?.guestCount   ?? null,
  checkIn:    props.reservation?.checkIn      ?? null,
  checkOut:   props.reservation?.checkOut     ?? null,
  grandTotal: props.reservation?.grandTotal   ?? null,
})

// ── DTO для create ──────────────────────────────────────────────────────────
const guestDtoCreate = ref<GuestDtoCreate>({
  name:  '',
  email: '',
})

const reservationDtoCreate = ref<ReservationDtoCreate>({
  roomId:     0,
  guestCount: 1,
  checkIn:    '',
  checkOut:   '',
  grandTotal: null,
})

// ── Локальна копія для відображення (view/edit) ─────────────────────────────
const localReservation = ref<ReservationFullDtoGet | null>(
    props.reservation ? { ...props.reservation } : null
)

function applyUpdate(patch: Partial<ReservationFullDtoGet>) {
  if (localReservation.value) {
    localReservation.value = { ...localReservation.value, ...patch }
  }
  // Синхронізуємо update-DTO з патчем
  if (patch.room?.id     !== undefined) reservationDtoUpdate.value.roomId     = patch.room.id
  if (patch.guestCount   !== undefined) reservationDtoUpdate.value.guestCount = patch.guestCount
  if (patch.checkIn      !== undefined) reservationDtoUpdate.value.checkIn    = patch.checkIn
  if (patch.checkOut     !== undefined) reservationDtoUpdate.value.checkOut   = patch.checkOut
  if (patch.grandTotal   !== undefined) reservationDtoUpdate.value.grandTotal = patch.grandTotal
}

// ── Заголовок ───────────────────────────────────────────────────────────────
const heading = computed(() => {
  switch (mode.value) {
    case 'view':
    case 'edit':
      return props.reservation ? 'Бронювання #' + props.reservation.id : 'Бронювання'
    case 'create':
      return 'Нове бронювання'
    default:
      return ''
  }
})

// ── API-дії ─────────────────────────────────────────────────────────────────
async function fetchRooms() {
  try {
    rooms.value = await RoomAPI.getAll() as RoomDtoGet[]
  } catch (e) {
    console.error('Failed to fetch rooms', e)
  }
}

async function toList() {
  await navigateTo('/admin/reservations')
}

async function edit() {
  mode.value = 'edit'
  await fetchRooms()
}

async function cancelEdit() {
  mode.value = 'view'
  // Скидаємо update-DTO до оригінальних значень
  reservationDtoUpdate.value = {
    roomId:     props.reservation?.room?.id     ?? null,
    guestCount: props.reservation?.guestCount   ?? null,
    checkIn:    props.reservation?.checkIn      ?? null,
    checkOut:   props.reservation?.checkOut     ?? null,
    grandTotal: props.reservation?.grandTotal   ?? null,
  }
  localReservation.value = props.reservation ? { ...props.reservation } : null
}

async function delete_() {
  if (!props.reservation) return
  try {
    await ReservationAPI.delete(props.reservation.id)
    await navigateTo('/admin/reservations')
  } catch (e) {
    console.error('Failed to delete', e)
    showError(e.message)
  }
}

async function save() {
  if (!props.reservation) return
  try {
    await ReservationAPI.update(props.reservation.id, reservationDtoUpdate.value)
    mode.value = 'view'
  } catch (e) {
    console.error('Failed to save', e)
    showError(e.message)
  }
}

async function create() {
  try {
    const result = await ReservationAPI.create(reservationDtoCreate.value, guestDtoCreate.value) as ReservationDtoGet
    await navigateTo(`/admin/reservations/${result.id}`)
  } catch (e) {
    console.error('Failed to create', e)
    showError(e.message)
  }
}


function showError(message: string) {
  errorMessage.value = message
  errorPopUpVisible.value = true
}
function showSuccess(message: string) {
  successMessage.value = message
  successPopUpVisible.value = true
}
// ── Монтування ──────────────────────────────────────────────────────────────
onMounted(() => {
  if (mode.value === 'create' || (mode.value === 'edit' && !rooms.value.length)) {
    fetchRooms()
  }
})
</script>

<template>
  <main>
    <div class="content">
      <PageHeader :heading :mode>
        <template #buttons>
          <!-- VIEW -->
          <button @click="toList" v-if="mode === 'view'" class="btn btn-ghost">До списку</button>
          <button @click="edit" v-if="mode === 'view' && reservation" class="btn btn-secondary">Редагувати</button>
          <button @click="delete_" v-if="mode === 'view' && reservation" class="btn btn-danger">Видалити</button>

          <!-- EDIT -->
          <button @click="save" v-if="mode === 'edit'" class="btn btn-primary">Зберегти</button>
          <button @click="cancelEdit" v-if="mode === 'edit'" class="btn btn-secondary">Скасувати</button>

          <!-- CREATE -->
          <button @click="create" v-if="mode === 'create'" class="btn btn-primary">Зберегти</button>
        </template>

        <template #secondary>
          <div class="page-meta" v-if="reservation">
            Створено: {{ reservation.createdAt }}
          </div>
        </template>
      </PageHeader>

      <div class="form-layout">
        <div class="col-left">
          <ReservationMainCard
              :reservation="localReservation"
              :mode
              :reservation-dto-update
              :reservation-dto-create
              @update:reservation="applyUpdate"
              @update:reservationDtoUpdate="v => reservationDtoUpdate = { ...reservationDtoUpdate, ...v }"
              @update:reservationDtoCreate="v => reservationDtoCreate = { ...reservationDtoCreate, ...v }"
          />
          <ReservationGuestCard
              :reservation="localReservation"
              :mode
              :guest-dto-create
              @update:reservation="applyUpdate"
              @update:guestDtoCreate="v => guestDtoCreate = { ...guestDtoCreate, ...v }"
          />
          <ReservationRoomCard
              :reservation="localReservation"
              :mode
              :available-rooms="rooms"
              :reservation-dto-update
              :reservation-dto-create
              @update:reservation="applyUpdate"
              @update:reservationDtoUpdate="v => reservationDtoUpdate = { ...reservationDtoUpdate, ...v }"
              @update:reservationDtoCreate="v => reservationDtoCreate = { ...reservationDtoCreate, ...v }"
          />
        </div>
      </div>
    </div>

    <PopUp title="Помилка" type="error" v-model="errorPopUpVisible">{{errorMessage}}</PopUp>

  </main>
</template>

<style scoped>
main {
  background: var(--background);
  flex: 1;
  padding: 2rem 1rem;
  min-height: 100dvh;
  display: flex;
}

.content {
  max-width: 800px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-layout {
  display: flex;
  gap: 2rem;
  margin-top: 2rem;
}

.col-left {
  flex: 2;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.col-right {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}
</style>