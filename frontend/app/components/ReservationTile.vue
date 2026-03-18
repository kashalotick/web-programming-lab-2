<script setup lang="ts">
import type {Reservation} from "~/types/reservation";
import {formatFullDate, formatShortDate} from "~/utils";

const props = defineProps<{
  reservation: Reservation
}>()

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}


const checkIn = computed(() => formatShortDate(props.reservation.checkIn))
const checkOut = computed(() => formatShortDate(props.reservation.checkOut))
const createdAt = computed(() => formatFullDate(props.reservation.createdAt))

defineEmits<{
  edit: [number],
  cancel: [number],
  delete: [number],
}>()
</script>

І Vue-компонент з Nuxt Icons:
vue
<template>
  <div class="sb-tile" :class="{ 'sb-tile--inactive': !reservation.isActive }">
    <div class="sb-tile-main">

      <div class="sb-tile-stripe"/>

      <div class="sb-tile-left">
        <div class="sb-tile-room">Кімната № {{ reservation.roomId }}</div>
        <div class="sb-tile-id">#RES-{{ String(reservation.id).padStart(5, '0') }}</div>
      </div>

      <div class="sb-tile-center">
        <div class="sb-tile-guest">
          <Icon name="lucide:user" class="sb-icon-accent"/>
          <!--          {{ reservation.guestName }} -->
          Guest name · <span class="sb-tile-phone">+380124124124</span>
        </div>
        <div class="sb-tile-row">
          <span class="sb-date-badge">{{ checkIn }}</span>
          <span class="sb-arrow">→</span>
          <span class="sb-date-badge">{{ checkOut }}</span>
          <span class="sb-meta">
            <Icon name="lucide:clock" class="sb-icon-muted"/>
            Створено {{ createdAt }}
          </span>
        </div>
      </div>

      <div class="sb-tile-right">
        <div class="sb-top-row">
          <div class="sb-price">
            <span class="sb-price-label">До сплати</span>
            <span class="sb-price-amount" :class="{ 'sb-price--inactive': !reservation.isActive }">
              ₴ {{ reservation.grandTotal }}
            </span>
          </div>
          <span class="sb-badge" :class="reservation.isActive ? 'sb-badge-active' : 'sb-badge-inactive'">
            {{ reservation.isActive ? 'Активне' : 'Неактивне' }}
          </span>
        </div>
        <div class="sb-actions">
          <button class="sb-btn sb-btn-edit" @click="$emit('edit', reservation.id)">
            <Icon name="lucide:pencil"/>
            Редагувати
          </button>
          <button class="sb-btn sb-btn-cancel" @click="$emit('cancel', reservation.id)">
            <Icon name="lucide:x-circle"/>
            Скасувати
          </button>
          <button class="sb-btn sb-btn-delete" @click="$emit('delete', reservation.id)">
            <Icon name="lucide:trash-2"/>
            Видалити
          </button>
        </div>
      </div>

    </div>
  </div>
</template>


<style scoped>
.sb-tile {
  background: var(--background);
  border: 1.5px solid var(--accent-light);
  border-radius: 16px;
  overflow: hidden;
  font-family: var(--font-description), serif;
  transition: box-shadow .2s;
  max-width: 900px;
  margin: 1rem auto;
}

.sb-tile:hover {
  box-shadow: 0 4px 24px rgba(210, 107, 46, .1);
}

.sb-tile-main {
  display: grid;
  grid-template-columns: 3px 180px 1fr auto;
  align-items: stretch;
  min-height: 80px;
}

.sb-tile-stripe {
  background: linear-gradient(180deg, var(--accent), var(--accent-dark));
  border-radius: 16px 0 0 0;
}

.sb-tile-left {
  padding: .9rem 1rem .9rem 1.1rem;
  border-right: 1px solid var(--accent-light);
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: .2rem;
}

.sb-tile-room {
  font-size: 1.25rem;
  font-weight: 500;
  color: var(--text);
  font-family: var(--font-heading), serif;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sb-tile-id {
  font-size: .75rem;
  color: var(--accent-dark);
  letter-spacing: .07em;
  text-transform: uppercase;
  font-family: var(--font-description), sans-serif;
  opacity: .85;
}

.sb-tile-center {
  padding: .9rem 1.2rem;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: .4rem;
}

.sb-tile-guest {
  font-size: 1.25rem;
  font-weight: 500;
  color: var(--text);
  display: flex;
  align-items: center;
  gap: .4rem;
  font-family: var(--font-heading), serif;
}
.sb-tile-phone {
  font-family: var(--font-description), sans-serif;
  font-size: 1rem;
  color: #888;
  margin-top: .25rem;
}
.sb-tile-guest svg {
  width: 14px;
  height: 14px;
  stroke: var(--accent-dark);
  fill: none;
  stroke-width: 1.8;
  stroke-linecap: round;
  flex-shrink: 0;
}

.sb-tile-row {
  display: flex;
  align-items: center;
  gap: .6rem;
  flex-wrap: wrap;
}

.sb-date-badge {
  background: var(--background-dark);
  border: 1px solid var(--accent-light);
  border-radius: 7px;
  padding: .2rem .55rem;
  font-size: .875rem;
  color: var(--accent-dark);
  font-weight: 600;
  font-family: var(--font-heading), sans-serif;
  white-space: nowrap;
}

.sb-arrow {
  color: var(--accent-light);
  font-size: .875rem;
  line-height: 1;
}

.sb-meta {
  font-size: .75rem;
  color: #888;
  display: flex;
  align-items: center;
  gap: .3rem;
  margin-left: .1rem;
}

.sb-meta svg {
  width: 11px;
  height: 11px;
  stroke: #888;
  fill: none;
  stroke-width: 1.8;
  stroke-linecap: round;
  flex-shrink: 0;
}

.sb-tile-right {
  padding: .9rem 1.1rem;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  justify-content: space-between;
  gap: .5rem;
}

.sb-badge {
  font-size: .75rem;
  font-weight: 700;
  letter-spacing: .09em;
  text-transform: uppercase;
  border-radius: 20px;
  padding: .28rem .7rem;
  white-space: nowrap;
  height: fit-content;
}

.sb-badge-active {
  background: var(--complementary-light);
  color: var(--complementary-dark);
}

.sb-badge-inactive {
  background: var(--gray);
  color: var(--text);
}

.sb-actions {
  display: flex;
  gap: .4rem;
  align-items: center;
}

.sb-btn {
  font-size: .75rem;
  font-weight: 700;
  letter-spacing: .07em;
  text-transform: uppercase;
  border-radius: 8px;
  padding: .38rem .7rem;
  cursor: pointer;
  border: 1.5px solid transparent;
  transition: background .15s, transform .1s;
  white-space: nowrap;
  font-family: var(--font-heading), serif;

}

.sb-top-row {
  display: flex;
  gap: .75rem;
}

.sb-price {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: .05rem;
}

.sb-price-label {
  font-size: .75rem;
  letter-spacing: .08em;
  color: #888;
  font-family: var(--font-description), sans-serif;
}

.sb-price-amount {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--accent-dark);
  font-family: var(--font-heading), serif;
  white-space: nowrap;
}


.sb-btn:active {
  transform: scale(.96);
}

.sb-btn-edit {
  background: var(--background-dark);
  color: var(--accent-dark);
  border-color: var(--accent-light);
}

.sb-btn-edit:hover {
  background: var(--accent-light);
}

.sb-btn-cancel {
  background: transparent;
  color: #888;
  border-color: var(--gray);
}

.sb-btn-cancel:hover {
  background: #f5f5f5;
}

.sb-btn-delete {
  background: transparent;
  color: #dc3545;
  border-color: #f5c0c0;
}

.sb-btn-delete:hover {
  background: #fff0f0;
}

</style>