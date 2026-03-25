<script setup lang="ts">
import type {
  ReservationFullDtoGet,
  ReservationDtoCreate,
  ReservationDtoUpdate,
  ReservationDtoGet
} from "~/types/reservation"
import type {GuestDtoCreate} from "~/types/guest"
import type {AvailabilityDtoGet, RoomDtoGet} from "~/types/room"
import {ReservationAPI} from "~/services/reservation-service"
import {RoomAPI} from "~/services/room-service"
import ReservationMainCard from "~/components/reservations/ReservationMainCard.vue"
import ReservationRoomCard from "~/components/reservations/ReservationRoomCard.vue"
import ReservationGuestCard from "~/components/reservations/ReservationGuestCard.vue"
import PopUp from "~/components/ui/PopUp.vue";
import {guestCreateSchema, reservationCreateSchema, reservationUpdateSchema} from "~/types/schemas/reservation-schema";
import {formatFullDate} from "~/utils";

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
}>()

const mode = ref<'view' | 'edit' | 'create'>(props.reservation ? 'view' : 'create')
const rooms = ref<RoomDtoGet[]>([])
const availability = ref<AvailabilityDtoGet>({})

// -- Pop ups --------------------------------------------------------
const errorMessage = ref<string>('')
const errorPopUpVisible = ref(false)


// ── DTO для edit ────────────────────────────────────────────────────────────
const reservationDtoUpdate = ref<ReservationDtoUpdate>({
  roomId: props.reservation?.room?.id ?? null,
  guestCount: props.reservation?.guestCount ?? null,
  checkIn: props.reservation?.checkIn ?? null,
  checkOut: props.reservation?.checkOut ?? null,
  grandTotal: props.reservation?.grandTotal ?? null,
})

// ── DTO для create ───────────────────────────────────────────────────────────
const guestDtoCreate = ref<GuestDtoCreate>({name: '', email: ''})

const reservationDtoCreate = ref<ReservationDtoCreate>({
  roomId: 0,
  guestCount: 1,
  checkIn: '',
  checkOut: '',
  grandTotal: null,
})

// ── Локальна копія для відображення ─────────────────────────────────────────
const localReservation = ref<ReservationFullDtoGet | null>(
    props.reservation ? {...props.reservation} : null
)

function applyUpdate(patch: Partial<ReservationFullDtoGet>) {
  if (localReservation.value) {
    localReservation.value = {...localReservation.value, ...patch}
  }
  if (patch.room?.id !== undefined) reservationDtoUpdate.value.roomId = patch.room.id
  if (patch.guestCount !== undefined) reservationDtoUpdate.value.guestCount = patch.guestCount
  if (patch.checkIn !== undefined) reservationDtoUpdate.value.checkIn = patch.checkIn
  if (patch.checkOut !== undefined) reservationDtoUpdate.value.checkOut = patch.checkOut
  if (patch.grandTotal !== undefined) reservationDtoUpdate.value.grandTotal = patch.grandTotal
}

// ── availability з "розблокованим" діапазоном поточного бронювання ───────────
// При edit picker отримує пропатчену версію: дати самого бронювання
// позначаються як доступні, щоб юзер міг їх знову обрати
const patchedAvailability = computed<AvailabilityDtoGet>(() => {
  // Тільки для edit і тільки коли кімната не змінилась
  const origCheckIn = props.reservation?.checkIn
  const origCheckOut = props.reservation?.checkOut
  const origRoomId = props.reservation?.room?.id
  const currentRoomId = reservationDtoUpdate.value.roomId ?? origRoomId

  if (
      mode.value !== 'edit' ||
      !origCheckIn || !origCheckOut ||
      currentRoomId !== origRoomId  // кімната змінилась — не патчимо
  ) {
    return availability.value
  }

  const patched = {...availability.value}

  // Ітеруємо всі дати від checkIn до checkOut (не включно) і розблоковуємо
  const cursor = new Date(origCheckIn)
  const end = new Date(origCheckOut)

  while (cursor < end) {
    const dateStr = cursor.toISOString().split('T')[0]!
    patched[dateStr] = {
      isAvailable: true,
      price: patched[dateStr]?.price ?? 0,
    }
    cursor.setDate(cursor.getDate() + 1)
  }

  return patched
})

// ── Поточний roomId (для watch) ──────────────────────────────────────────────
const activeRoomId = computed(() => {
  if (mode.value === 'create') return reservationDtoCreate.value.roomId || null
  if (mode.value === 'edit') return reservationDtoUpdate.value.roomId ?? localReservation.value?.room?.id ?? null
  return localReservation.value?.room?.id ?? null
})

// При зміні кімнати — скидаємо дати та підвантажуємо availability
watch(activeRoomId, async (newId, oldId) => {
  if (!newId || newId === oldId) return

  if (mode.value === 'create') {
    await fetchAvailability(newId)
    reservationDtoCreate.value.checkIn  = ''
    reservationDtoCreate.value.checkOut = ''
  } else if (mode.value === 'edit') {
    await fetchAvailability(newId)

    const origIn  = props.reservation?.checkIn  ?? null
    const origOut = props.reservation?.checkOut ?? null

    const datesAvailable = origIn && origOut
        ? areDatesAvailable(origIn, origOut, availability.value)
        : false

    const ci  = datesAvailable ? origIn  : null
    const co  = datesAvailable ? origOut : null

    reservationDtoUpdate.value.checkIn  = ci
    reservationDtoUpdate.value.checkOut = co
    if (localReservation.value) {
      localReservation.value = { ...localReservation.value, checkIn: ci ?? '', checkOut: co ?? '' }
    }
  }

  // fetchAvailability(newId)
})
function areDatesAvailable(checkIn: string, checkOut: string, avail: AvailabilityDtoGet): boolean {
  const cursor = new Date(checkIn)
  const end    = new Date(checkOut)
  while (cursor < end) {
    const dateStr = cursor.toISOString().split('T')[0]!
    if (!avail[dateStr]?.isAvailable) return false
    cursor.setDate(cursor.getDate() + 1)
  }
  return true
}

// ── Заголовок ────────────────────────────────────────────────────────────────
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

// ── API ──────────────────────────────────────────────────────────────────────
async function fetchRooms() {
  try {
    rooms.value = await RoomAPI.getAll() as RoomDtoGet[]
  } catch (e) {
    console.error('Failed to fetch rooms', e)
  }
}

async function fetchAvailability(roomId: number) {
  const from = new Date()
  const to = new Date()
  to.setDate(to.getDate() + 90)
  try {
    availability.value = await RoomAPI.getAvailability(roomId, {
      from: from.toISOString().split('T')[0],
      to: to.toISOString().split('T')[0],
    }) as AvailabilityDtoGet
  } catch (e) {
    console.error('Failed to fetch availability', e)
  }
}

async function toList() {
  await navigateTo('/admin/reservations')
}

async function edit() {
  mode.value = 'edit'
  await fetchRooms()
  const rid = localReservation.value?.room?.id
  if (rid) fetchAvailability(rid)
}

async function cancelEdit() {
  mode.value = 'view'
  reservationDtoUpdate.value = {
    roomId: props.reservation?.room?.id ?? null,
    guestCount: props.reservation?.guestCount ?? null,
    checkIn: props.reservation?.checkIn ?? null,
    checkOut: props.reservation?.checkOut ?? null,
    grandTotal: props.reservation?.grandTotal ?? null,
  }
  localReservation.value = props.reservation ? {...props.reservation} : null
  availability.value = {}
}

async function delete_() {
  if (!props.reservation) return
  if (!confirm('Ви впевнені, що хочете видалити це бронювання?')) return
  try {
    await ReservationAPI.delete(props.reservation.id)
    await navigateTo('/admin/reservations')
  } catch (e) {
    showError(e)
  }
}

async function save() {
  if (!props.reservation) return

  const parsed = reservationUpdateSchema.safeParse(reservationDtoUpdate.value)
  if (!parsed.success) {
    showErrorMessage(parsed.error.errors[0]?.message ?? 'Unknown error')
    return
  }

  console.log('Saving with DTO', parsed.data)
  try {
    await ReservationAPI.update(props.reservation.id, parsed.data)
    mode.value = 'view'
    availability.value = {}

    const updated = await ReservationAPI.getFull(props.reservation.id) as ReservationFullDtoGet
    localReservation.value = updated
  } catch (e) {
    showError(e)
  }
}

async function create() {
  const parsedReservation = reservationCreateSchema.safeParse(reservationDtoCreate.value)
  if (!parsedReservation.success) {
    showErrorMessage(parsedReservation.error.errors[0]?.message ?? 'Unknown error')
    return
  }

  const parsedGuest = guestCreateSchema.safeParse(guestDtoCreate.value)
  if (!parsedGuest.success) {
    showErrorMessage(parsedGuest.error.errors[0]?.message ?? 'Unknown error')
    return
  }

  console.log('Creating with DTO', parsedReservation.data, parsedGuest.data)
  try {
    const result = await ReservationAPI.create(parsedReservation.data, parsedGuest.data) as ReservationDtoGet
    await navigateTo(`/admin/reservations/${result.id}`)
  } catch (e) {
    showError(e)
  }
}
async function cancelReservation(id: number | undefined) {
  if (!id) return
  if (!confirm('Ви впевнені, що хочете скасувати це бронювання?')) return
  if (!props.reservation?.isActive) return
  try {
    await ReservationAPI.cancel(id)
    console.log(`Cancel reservation ${id}`)
    if (localReservation.value) {
      localReservation.value.isActive = false
    }
  } catch (e) {
    showError(e)
  }

}

function showErrorMessage(message: string) {
  errorMessage.value = message
  errorPopUpVisible.value = true
}
function showError(err: Error | any) {
  console.error('Failed to create', err)
  console.error(err.data)
  const errMessage = extractErrorMessage(err.data)
  console.error(errMessage)

  errorMessage.value = errMessage
  errorPopUpVisible.value = true
}

// ── Монтування ──────────────────────────────────────────────────────────────
onMounted(() => {
  if (mode.value === 'create' || (mode.value === 'edit' && !rooms.value.length)) {
    fetchRooms()
  }
})
function extractErrorMessage(errData: any) {
  if (!errData) return 'Unknown error';

  if (errData.errors && typeof errData.errors === 'object') {
    const messages = Object.values(errData.errors).flat();
    if (messages.length > 0) return messages.join('; ');
  }
  if (typeof errData.error === 'string') {
    return errData.error;
  }

  return 'Unknown error';
}

</script>

<template>
  <main>
    <div class="content">
      <PageHeader :heading :mode>
        <template #buttons>
          <button @click="toList" v-if="mode === 'view'" class="btn btn-ghost">До списку</button>
          <button @click="edit" v-if="mode === 'view' && reservation?.isActive" class="btn btn-secondary">Редагувати</button>
          <button @click="delete_" v-if="mode === 'view' && reservation" class="btn btn-danger">Видалити</button>
          <button @click="save" v-if="mode === 'edit'" class="btn btn-primary">Зберегти</button>
          <button @click="cancelEdit" v-if="mode === 'edit'" class="btn btn-secondary">Скасувати</button>
          <button @click="create" v-if="mode === 'create'" class="btn btn-primary">Зберегти</button>
          <button @click="toList" v-if="mode === 'create'" class="btn btn-ghost">Скасувати</button>
        </template>
        <template #secondary>
          <div class="page-meta" v-if="reservation">
            Створено: {{ formatFullDate(new Date(reservation.createdAt)) }}
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
              :availability="patchedAvailability"
              :original-check-in="props.reservation?.checkIn ?? null"
              :original-check-out="props.reservation?.checkOut ?? null"
              @update:reservation="applyUpdate"
              @update:reservationDtoUpdate="v => reservationDtoUpdate = { ...reservationDtoUpdate, ...v }"
              @update:reservationDtoCreate="v => reservationDtoCreate = { ...reservationDtoCreate, ...v }"
              @cancel="cancelReservation"
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

    <PopUp title="Помилка" type="error" v-model="errorPopUpVisible">{{ errorMessage }}</PopUp>

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
  width: 100%;
  max-width: 820px;
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