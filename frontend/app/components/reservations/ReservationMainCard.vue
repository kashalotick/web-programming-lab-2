<script setup lang="ts">
import type { ReservationFullDtoGet, ReservationDtoUpdate, ReservationDtoCreate } from '~/types/reservation'
import type { AvailabilityDtoGet } from '~/types/room'
import ReservationDatePicker from '~/components/ui/ReservationDatePicker.vue'

const MAX_STAY = 14

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
  mode: 'view' | 'edit' | 'create'
  reservationDtoUpdate?: ReservationDtoUpdate
  reservationDtoCreate?: ReservationDtoCreate
  availability?: AvailabilityDtoGet
  // Оригінальні дати бронювання — для відновлення при скиданні в edit-режимі
  originalCheckIn?: string | null
  originalCheckOut?: string | null
}>()

const emit = defineEmits<{
  (e: 'cancel', id: number | undefined): void
  (e: 'update:reservation', val: Partial<ReservationFullDtoGet>): void
  (e: 'update:reservationDtoUpdate', val: Partial<ReservationDtoUpdate>): void
  (e: 'update:reservationDtoCreate', val: Partial<ReservationDtoCreate>): void
}>()

const editable = computed(() => props.mode === 'edit' || props.mode === 'create')

// ── checkIn ──────────────────────────────────────────────────────────────────
const checkInValue = computed<string | null>({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.checkIn || null
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.checkIn ?? props.reservation?.checkIn ?? null
    return props.reservation?.checkIn ?? null
  },
  set: (v) => setCheckIn(v),
})

// ── checkOut ─────────────────────────────────────────────────────────────────
const checkOutValue = computed<string | null>({
  get: () => {
    if (props.mode === 'create') return props.reservationDtoCreate?.checkOut || null
    if (props.mode === 'edit')   return props.reservationDtoUpdate?.checkOut ?? props.reservation?.checkOut ?? null
    return props.reservation?.checkOut ?? null
  },
  set: (v) => setCheckOut(v),
})

function setCheckIn(v: string | null) {
  // null від picker-а (скидання) → повертаємо оригінальне значення в edit, '' в create
  const val = v ?? (props.mode === 'edit' ? (props.originalCheckIn ?? '') : '')
  if (props.mode === 'create') emit('update:reservationDtoCreate', { checkIn: val })
  else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { checkIn: val || null })
  emit('update:reservation', { checkIn: val })
}

function setCheckOut(v: string | null) {
  const val = v ?? (props.mode === 'edit' ? (props.originalCheckOut ?? '') : '')
  if (props.mode === 'create') emit('update:reservationDtoCreate', { checkOut: val })
  else if (props.mode === 'edit') emit('update:reservationDtoUpdate', { checkOut: val || null })
  emit('update:reservation', { checkOut: val })
}

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
    emit('update:reservation', { grandTotal: v ?? undefined })
  },
})
function calcGrandTotal(checkIn: string | null, checkOut: string | null): number | null {
  if (!checkIn || !checkOut || !props.availability) return null
  const nights = (new Date(checkOut).getTime() - new Date(checkIn).getTime()) / (1000 * 60 * 60 * 24)
  if (nights <= 0) return null
  const price = props.availability[checkIn]?.price ?? null
  if (price == null) return null
  return price * nights
}

watch(
    () => [checkInValue.value, checkOutValue.value] as const,
    ([ci, co]) => {
      if (!editable.value) return
      const total = calcGrandTotal(ci, co)
      if (total !== null) grandTotal.value = total
    },
)
// ── DatePicker state ─────────────────────────────────────────────────────────
const activeField = ref<'checkIn' | 'checkOut' | null>(null)
const inRef  = useTemplateRef<HTMLElement>('checkInWrapper')
const outRef = useTemplateRef<HTMLElement>('checkOutWrapper')
const inInputRef  = useTemplateRef<HTMLInputElement>('checkInInput')
const outInputRef = useTemplateRef<HTMLInputElement>('checkOutInput')

watch(activeField, (v) => {
  if (v === 'checkIn')  inInputRef.value?.focus()
  if (v === 'checkOut') outInputRef.value?.focus()
  if (!v) { inInputRef.value?.blur(); outInputRef.value?.blur() }
})

watch(activeField, (v) => {
  if (v === 'checkIn')  inInputRef.value?.focus()
  if (v === 'checkOut') outInputRef.value?.focus()
  if (!v) { inInputRef.value?.blur(); outInputRef.value?.blur() }
})
// ── Форматування ─────────────────────────────────────────────────────────────
function format(date: string | null | undefined) {
  if (!date) return ''
  return new Date(date).toLocaleDateString('uk-UA')
}

// ── isActive ─────────────────────────────────────────────────────────────────
async function cancelReservation() {
  emit('cancel', props.reservation?.id)
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
          <template v-if="editable">
            <div class="calendar-input-wrapper" ref="checkInWrapper">
              <input
                  ref="checkInInput"
                  :value="format(checkInValue)"
                  @focus="activeField = 'checkIn'"
                  readonly
                  placeholder="дд.мм.рррр"
              />
            </div>
          </template>
          <template v-else>
            <input :value="format(checkInValue)" readonly/>
          </template>
        </div>

        <div class="sb-field">
          <label>Виїзд <span class="required">*</span></label>
          <template v-if="editable">
            <div class="calendar-input-wrapper" ref="checkOutWrapper">
              <input
                  ref="checkOutInput"
                  :value="format(checkOutValue)"
                  @focus="activeField = 'checkOut'"
                  readonly
                  placeholder="дд.мм.рррр"
              />
            </div>
          </template>
          <template v-else>
            <input :value="format(checkOutValue)" readonly/>
          </template>
        </div>
      </div>

      <ReservationDatePicker
          v-if="editable"
          v-model:activeField="activeField"
          v-model:checkIn="checkInValue"
          v-model:checkOut="checkOutValue"
          :check-in-target="inRef"
          :check-out-target="outRef"
          :dates="availability ?? {}"
          :max-stay="MAX_STAY"
      />

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
          <button @click="cancelReservation" v-if="reservation.isActive" class="btn btn-secondary">
            Скасувати бронювання
          </button>
        </div>
      </div>

    </div>
  </div>
</template>

<style scoped>
.calendar-input-wrapper {
  position: relative;
}
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
</style>