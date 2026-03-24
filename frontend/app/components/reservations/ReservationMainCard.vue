<script setup lang="ts">
import type { ReservationFullDtoGet, ReservationDtoUpdate, ReservationDtoCreate } from '~/types/reservation'

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
  mode: 'view' | 'edit' | 'create'
  reservationDtoUpdate?: ReservationDtoUpdate
  reservationDtoCreate?: ReservationDtoCreate
}>()

const emit = defineEmits<{
  (e: 'update:reservation', val: Partial<ReservationFullDtoGet>): void
  (e: 'update:reservationDtoUpdate', val: Partial<ReservationDtoUpdate>): void
  (e: 'update:reservationDtoCreate', val: Partial<ReservationDtoCreate>): void
}>()

const editable = computed(() => props.mode === 'edit' || props.mode === 'create')

// ── checkIn ─────────────────────────────────────────────────────────────────
const checkIn = computed({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.checkIn ?? ''
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.checkIn ?? props.reservation?.checkIn ?? ''
    return props.reservation?.checkIn ?? ''
  },
  set: (v) => {
    if (props.mode === 'create') emit('update:reservationDtoCreate', { checkIn: v })
    else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { checkIn: v })
    emit('update:reservation', { checkIn: v })
  },
})

// ── checkOut ────────────────────────────────────────────────────────────────
const checkOut = computed({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.checkOut ?? ''
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.checkOut ?? props.reservation?.checkOut ?? ''
    return props.reservation?.checkOut ?? ''
  },
  set: (v) => {
    if (props.mode === 'create') emit('update:reservationDtoCreate', { checkOut: v })
    else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { checkOut: v })
    emit('update:reservation', { checkOut: v })
  },
})

// ── grandTotal ───────────────────────────────────────────────────────────────
const grandTotal = computed({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.grandTotal ?? null
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.grandTotal ?? props.reservation?.grandTotal ?? null
    return props.reservation?.grandTotal ?? null
  },
  set: (v) => {
    if (props.mode === 'create') emit('update:reservationDtoCreate', { grandTotal: v })
    else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { grandTotal: v })
    emit('update:reservation', { grandTotal: v })
  },
})

// ── isActive (тільки view/edit, не входить у DTO) ───────────────────────────
const isActive = computed({
  get: () => props.reservation?.isActive ?? true,
  set: (v) => emit('update:reservation', { isActive: v }),
})

async function cancelReservation() {
  if (!props.reservation?.isActive) return
  if (!confirm('Ви впевнені, що хочете скасувати це бронювання?')) return
  emit('update:reservation', { isActive: false })
}
</script>

<template>
  <div class="card card--accented">
    <div class="card-header">
      <span class="card-header-icon">📋</span> Основні дані
    </div>
    <div class="card-body">

      <div class="field-row field">
        <div class="sb-field">
          <label>Заїзд <span class="required">*</span></label>
          <input type="date" v-model="checkIn" :readonly="!editable"/>
        </div>
        <div class="sb-field">
          <label>Виїзд <span class="required">*</span></label>
          <input type="date" v-model="checkOut" :readonly="!editable"/>
        </div>
      </div>

      <div class="sb-field">
        <label>Загальна сума</label>
        <template v-if="editable">
          <input type="number" v-model="grandTotal" min="0" step="0.01"/>
        </template>
        <template v-else>
          <div class="computed">
            <span class="computed-label">Grand Total</span>
            <span class="computed-value">{{ reservation?.grandTotal ?? '—' }} ₴</span>
          </div>
        </template>
      </div>

      <!-- Статус — тільки для існуючих бронювань -->
      <div class="sb-field" v-if="mode !== 'create' && reservation">
        <label>Статус резервації</label>
        <div class="cancel-reservation">
          <div>
            <span
                class="badge"
                :class="{
                  'badge-active':   reservation.isActive,
                  'badge-inactive': !reservation.isActive,
                }"
            >
              <span class="badge-dot"></span>
              {{ reservation.isActive ? 'Активне' : 'Скасоване' }}
            </span>
          </div>
          <button
              @click="cancelReservation"
              v-if="reservation.isActive"
              class="btn btn-secondary"
          >
            Скасувати бронювання
          </button>
        </div>
      </div>

    </div>
  </div>
</template>

<style scoped>
.cancel-reservation {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}

.badge {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: .22rem .65rem;
  border-radius: 20px;
  font-size: .62rem;
  font-weight: 700;
  letter-spacing: .07em;
  text-transform: uppercase;
  font-family: var(--font-description), monospace;
}

.badge-active {
  background: var(--complementary-light);
  color: var(--complementary-dark);
}

.badge-inactive {
  background: #fde8e8;
  color: #a33;
}

.badge-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: currentColor;
  flex-shrink: 0;
}
input[readonly], select[disabled] {
  opacity: .6;
  cursor: not-allowed;
}
</style>