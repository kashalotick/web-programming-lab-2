<script setup lang="ts">
import type {Reservation} from "~/types/reservation";
import {formatFullDate, formatShortDate} from "~/utils";

const {reservation} = defineProps<{
  reservation: Reservation,
}>()

const emit = defineEmits<{
  view: [number],
  cancel: [number],
  delete: [number],
}>()

function onView() {
  emit('view', reservation.id)
}
function onCancel() {
  emit('cancel', reservation.id)
}
function onDelete() {
  emit('delete', reservation.id)
}
</script>

<template>
  <tr>
    <td class="td-id">#{{ reservation.id }}</td>
    <td><span class="chip"><span class="chip-label">guest&nbsp;</span>{{ reservation.guestId }}</span></td>
    <td><span class="chip"><span class="chip-label">room&nbsp;</span>{{ reservation.roomId }}</span></td>
    <td>
      <div class="td-dates">
        <span class="date-val">{{ formatShortDate(reservation.checkIn) }}</span>
        <span class="date-sep"> → </span>
        <span class="date-val">{{ formatShortDate(reservation.checkOut) }}</span>
      </div>
    </td>
    <td class="td-price">{{ reservation.grandTotal }} ₴</td>
    <td><span class="badge badge-active"><span
        class="badge-dot"></span>{{ reservation.isActive ? 'Активне' : 'Скасоване' }}</span></td>
    <td style="width: 12%">
      <span class="date-val">{{ formatFullDate(reservation.createdAt) }}</span>
    </td>
    <td class="fit">
      <div class="actions">
        <button @click="onView" class="btn btn-secondary btn-sm">Переглянути</button>
        <button v-if="reservation.isActive" @click="onCancel" class="btn btn-ghost btn-sm">Скасувати</button>
        <button v-if="!reservation.isActive" @click="onDelete" class="btn btn-danger btn-sm">Видалити</button>
      </div>
    </td>
  </tr>

</template>

<style scoped>
tr {
  border-bottom: 1px solid var(--background-dark);
  transition: background .12s;
}

tr:last-child {
  border-bottom: none;
}

tr:hover {
  background: var(--background);
}

td {
  padding: .75rem 1rem;
  vertical-align: middle;
  color: var(--text);
  white-space: nowrap;
  font-family: var(--font-description), sans-serif;
  font-weight: 300;

  &.fit {
    width: 1%;
  }
}

.td-id {
  font-family: var(--font-description), monospace;
  font-size: .75rem;
  color: var(--accent-dark);
  font-weight: 600;
  letter-spacing: .04em;
}

/* entity chip */
.chip {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: var(--background-dark);
  border: 1px solid var(--accent-light);
  border-radius: 6px;
  padding: .2rem .55rem;
  font-size: .72rem;
  font-family: var(--font-description), monospace;
  color: var(--accent-dark);
}

.chip-label {
  font-size: .58rem;
  color: var(--accent);
  text-transform: uppercase;
  letter-spacing: .06em;
}

/* dates */
.td-dates {
  display: flex;
  align-items: center;
  gap: .4rem;
}

.date-val {
  font-size: .78rem;
  color: var(--text);
}

.date-sep {
  color: var(--accent);
  font-size: .75rem;
}

/* price */
.td-price {
  font-family: var(--font-description), monospace;
  font-size: .82rem;
  font-weight: 600;
  color: var(--accent-dark);
}

/* badge */
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

.actions {
  display: flex;
  gap: 5px;
}
</style>