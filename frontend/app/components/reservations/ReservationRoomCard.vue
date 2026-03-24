<script setup lang="ts">
import type { ReservationFullDtoGet, ReservationDtoUpdate, ReservationDtoCreate } from '~/types/reservation'
import type { RoomDtoGet } from '~/types/room'

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
  availableRooms: RoomDtoGet[]
  mode: 'view' | 'edit' | 'create'
  reservationDtoUpdate?: ReservationDtoUpdate
  reservationDtoCreate?: ReservationDtoCreate
}>()

const emit = defineEmits<{
  (e: 'update:reservation', val: Partial<ReservationFullDtoGet>): void
  (e: 'update:reservationDtoUpdate', val: Partial<ReservationDtoUpdate>): void
  (e: 'update:reservationDtoCreate', val: Partial<ReservationDtoCreate>): void
}>()

const canSelectRoom = computed(() => props.mode === 'edit' || props.mode === 'create')

// ── Вибрана кімната ──────────────────────────────────────────────────────────
const selectedRoomId = computed({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.roomId ?? 0
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.roomId ?? props.reservation?.room?.id ?? 0
    return props.reservation?.room?.id ?? 0
  },
  set: (id: number) => {
    const found = props.availableRooms.find(r => r.id === id)
    if (!found) return

    if (props.mode === 'create') emit('update:reservationDtoCreate', { roomId: id })
    else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { roomId: id })

    emit('update:reservation', { room: found })
  },
})

// Кімната для відображення (береться з localReservation або зі списку по selectedRoomId)
const displayRoom = computed(() =>
    props.reservation?.room
    ?? props.availableRooms.find(r => r.id === selectedRoomId.value)
    ?? null
)
</script>

<template>
  <div class="card">
    <div class="card-header">
      <span class="card-header-icon">🛏</span> Кімната
    </div>
    <div class="card-body">

      <div class="field" v-if="canSelectRoom">
        <label>Кімната <span class="required">*</span></label>
        <select v-model="selectedRoomId">
          <option disabled :value="0">— Оберіть кімнату —</option>
          <option v-for="room in availableRooms" :key="room.id" :value="room.id">
            {{ room.name }} (id: {{ room.id }})
          </option>
        </select>
      </div>

      <template v-if="displayRoom">
        <div class="room-image">
          <img
              v-if="displayRoom.imageUrl"
              :src="displayRoom.imageUrl"
              :alt="displayRoom.name ?? 'Кімната'"
          />
          <div v-else class="room-img-placeholder">
            🖼 &nbsp;Зображення відсутнє
          </div>
        </div>

        <div class="entity-card">
          <div class="entity-card-title">
            Дані кімнати з БД
            <span class="entity-card-id">room #{{ displayRoom.id }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">id</span>
            <span class="entity-val mono">{{ displayRoom.id }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">name</span>
            <span class="entity-val">{{ displayRoom.name ?? '—' }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">capacity</span>
            <span class="entity-val">{{ displayRoom.capacity }} ос.</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">price/night</span>
            <span class="entity-val mono">{{ displayRoom.price }} ₴</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">description</span>
            <span class="entity-val entity-val--muted">{{ displayRoom.description ?? '—' }}</span>
          </div>
        </div>
      </template>

      <!-- Кімната ще не обрана (create) -->
      <template v-else-if="mode === 'create'">
        <div class="room-img-placeholder room-image">
          🛏 &nbsp;Оберіть кімнату зі списку
        </div>
      </template>

      <!-- Немає даних (fallback) -->
      <template v-else>
        <p class="text-muted">Дані кімнати недоступні</p>
      </template>

    </div>
  </div>
</template>

<style scoped>
.required { color: var(--error); margin-left: 2px; }

.room-image { margin-bottom: .9rem; }
.room-image img {
  width: 100%;
  aspect-ratio: 16/7;
  object-fit: cover;
  border-radius: 10px;
  border: 1.5px solid var(--accent-light);
}
.room-img-placeholder {
  width: 100%;
  aspect-ratio: 16/7;
  background: var(--background-dark);
  border: 1.5px dashed var(--accent-light);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--gray-dark);
  font-size: .8rem;
  gap: .4rem;
}

.text-muted {
  color: var(--gray-dark);
  font-size: .85rem;
}
input[readonly], select[disabled] {
  opacity: .6;
  cursor: not-allowed;
}
</style>