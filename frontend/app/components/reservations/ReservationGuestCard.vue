<script setup lang="ts">
import type { ReservationFullDtoGet } from '~/types/reservation'
import type { GuestDtoCreate } from '~/types/guest'

const props = defineProps<{
  reservation: ReservationFullDtoGet | null
  mode: 'view' | 'edit' | 'create'
  guestDtoCreate?: GuestDtoCreate
}>()

const emit = defineEmits<{
  (e: 'update:reservation', val: Partial<ReservationFullDtoGet>): void
  (e: 'update:guestDtoCreate', val: Partial<GuestDtoCreate>): void
}>()

// ── Поля для create-режиму (прив'язані до GuestDtoCreate) ────────────────────
const guestName = computed({
  get: () => props.guestDtoCreate?.name ?? '',
  set: (v) => emit('update:guestDtoCreate', { name: v }),
})

const guestEmail = computed({
  get: () => props.guestDtoCreate?.email ?? '',
  set: (v) => emit('update:guestDtoCreate', { email: v }),
})
</script>

<template>
  <div class="card">
    <div class="card-header">
      <span class="card-header-icon">👤</span> Гість
    </div>
    <div class="card-body">

      <!-- create: вводимо імʼя та email вручну через GuestDtoCreate -->
      <template v-if="mode === 'create'">
        <div class="field">
          <label>Імʼя <span class="required">*</span></label>
          <input type="text" v-model="guestName" placeholder="Введіть імʼя гостя"/>
        </div>
        <div class="field">
          <label>Email <span class="required">*</span></label>
          <input type="email" v-model="guestEmail" placeholder="email@example.com"/>
        </div>
      </template>

      <!-- view / edit: показуємо дані гостя з БД -->
      <template v-else-if="reservation">
        <div class="entity-card">
          <div class="entity-card-title">
            Дані гостя з БД
            <span class="entity-card-id">guest #{{ reservation.guest.id }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">id</span>
            <span class="entity-val mono">{{ reservation.guest.id }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">name</span>
            <span class="entity-val">{{ reservation.guest.name ?? '—' }}</span>
          </div>
          <div class="entity-row">
            <span class="entity-key">email</span>
            <span class="entity-val mono">{{ reservation.guest.email ?? '—' }}</span>
          </div>
        </div>
      </template>

      <!-- fallback якщо reservation null у view/edit -->
      <template v-else>
        <p class="text-muted">Дані гостя недоступні</p>
      </template>

    </div>
  </div>
</template>

<style scoped>
.text-muted {
  color: var(--gray-dark);
  font-size: .85rem;
}
</style>