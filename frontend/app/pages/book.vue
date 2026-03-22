<script setup lang="ts">
import {useForm, useFormValues} from 'vee-validate'
import {toTypedSchema} from '@vee-validate/zod'
import {z} from 'zod'
import type {Room} from "~/types/room";
import DateRangePicker from "~/components/DateRangePicker.vue";

const config = useRuntimeConfig()
const rooms = ref<Record<number, Room>>({})

const availability = ref<Record<string, {
  price: number;
  available: boolean;
}>>({})
const MAX_STAY = 7

fetchRooms()


const buildSchema = (maxCapacity: number) => toTypedSchema(z.object({
  roomId: z.number().min(1, 'Номер кімнати обовʼязковий'),
  checkIn: z.string()
      .min(1, 'Дата заїзду обовʼязкова')
      .refine(val => new Date(val) >= new Date(new Date().toDateString()), 'Дата не може бути в минулому'),
  checkOut: z.string()
      .min(1, 'Дата виїзду обовʼязкова'),
  name: z.string().min(1, 'Імʼя обовʼязкове'),
  phone: z.string().regex(/^\+?[\d\s\-\(\)]{10,15}$/, 'Невалідний номер телефону'),
  guests: z.number()
      .min(1, 'Мінімум 1 гість')
      .max(maxCapacity, `Максимум ${maxCapacity} гостей`),
}).refine(data => new Date(data.checkOut) > new Date(data.checkIn), {
  message: 'Дата виїзду має бути пізніше дати заїзду',
  path: ['checkOut'],
}))

const validationSchema = ref(buildSchema(10))

const {handleSubmit, defineField, errors} = useForm({validationSchema})


const [roomId, roomIdAttrs] = defineField('roomId')
const [checkIn, checkInAttrs] = defineField('checkIn')
const [checkOut, checkOutAttrs] = defineField('checkOut')
const [name, nameAttrs] = defineField('name')
const [phone, phoneAttrs] = defineField('phone') // TODO: replace with email
const [guests, guestsAttrs] = defineField('guests')

const nights = ref(0)

const grandTotal = computed(() => {
  if (!roomId.value) return 0
  var room = rooms.value[roomId.value];
  if (!room) return 0;
  return room.price * nights.value;
})


const onSubmit = handleSubmit(values => {
  console.log(values)
})

const activeField = ref<'checkIn' | 'checkOut' | null>(null)
const checkInModelValue = ref<string | null>(null)
const checkOutModelValue = ref<string | null>(null)
const inRef = useTemplateRef<HTMLElement>('checkIn')
const outRef = useTemplateRef<HTMLElement>('checkOut')
const inInputRef = useTemplateRef<HTMLElement>('checkInInput')
const outInputRef = useTemplateRef<HTMLElement>('checkOutInput')

watch(activeField, (newVal) => {
  if (newVal === 'checkIn') {
    inInputRef.value?.focus()
  }
  if (newVal === 'checkOut') {
    outInputRef.value?.focus()
  }
  if (!newVal) {
    inInputRef.value?.blur()
    outInputRef.value?.blur()
  }
})
watch(checkInModelValue, (newVal) => {
  checkIn.value = newVal == null ? '' : newVal
})
watch(checkOutModelValue, (newVal) => {
  checkOut.value = newVal == null ? '' : newVal
})
watch([checkIn, checkOut], ([ci, co]) => {
  if (!ci || !co) return nights.value = 0
  nights.value = (new Date(co).getTime() - new Date(ci).getTime()) / (1000 * 60 * 60 * 24)
})
watch(roomId, (newVal) => {
  if (newVal) {
    fetchAvailability(newVal)
  }
})

async function fetchRooms() {
  const result = await $fetch('rooms', {
    baseURL: config.public.apiBase,
  }) as Room[];
  rooms.value = Object.fromEntries(result.map(r => [r.id, r]))
}

async function fetchAvailability(roomId: number) {
  console.log(`Fetching availability for room ${roomId}...`)
  const dates: Record<string, {
    price: number;
    available: boolean;
  }> = {
    '2026-03-16': {price: 1200, available: true},
    '2026-03-17': {price: 1200, available: true},
    '2026-03-18': {price: 1200, available: true},
    '2026-03-19': {price: 1200, available: true},
    '2026-03-20': {price: 1200, available: true},

    '2026-03-21': {price: 1200, available: true},
    '2026-03-22': {price: 1200, available: true},
    '2026-03-23': {price: 1200, available: true},
    '2026-03-24': {price: 1200, available: true},
    '2026-03-25': {price: 1200, available: true},
    '2026-03-26': {price: 1200, available: true},
    '2026-03-27': {price: 1200, available: true},
    '2026-03-28': {price: 1200, available: true},
    '2026-03-29': {price: 1200, available: true},
    '2026-03-30': {price: 1200, available: true},
    '2026-03-31': {price: 1200, available: true},

    '2026-04-01': {price: 1200, available: true},
    '2026-04-02': {price: 1200, available: true},
    '2026-04-03': {price: 1200, available: true},
  }
  availability.value = dates
}

function format(date: string | null) {
  if (!date) return ''
  const dateObj = new Date(date)
  return dateObj.toLocaleDateString('uk-UA')
}
</script>

<template>
  <form class="sb-wrap" @submit="onSubmit">
    <div class="sb-card">
      <div class="sb-header">
        <div class="sb-header-leaf">🌿</div>
        <h2>Створити бронювання</h2>
        <!--        <p><span class="sb-accent-dot"></span>Еко-садиба СтавБір · Чернівецька обл.</p>-->
      </div>

      <div class="sb-body">

        <div class="sb-field">
          <label>Номер / Кімната</label>
          <select v-model="roomId" v-bind="roomIdAttrs">
            <option disabled value="">Оберіть кімнату</option>
            <option v-for="room in rooms" :key="room.id" :value="room.id">
              {{ room.name }} (до {{ room.capacity }} гостей)
            </option>
          </select>
          <!--          <select>-->
          <!--            <option disabled selected value="">Оберіть кімнату</option>-->
          <!--            <option>Лісова (до 2 гостей)</option>-->
          <!--            <option>Озерна (до 4 гостей)</option>-->
          <!--            <option>Садова (до 6 гостей)</option>-->
          <!--          </select>-->
          <span class="sb-error">{{ errors.roomId }}</span>
        </div>

        <div>
          <div class="sb-dates-label" style="margin-bottom:.5rem">Дати перебування</div>
          <div class="sb-row">
            <div class="sb-field">
              <label>Заїзд</label>
              <div class="calendar-input-wrapper" id="checkIn" ref="checkIn">
                <input ref="checkInInput" @focus="activeField = 'checkIn'" :value="format(checkInModelValue)"
                       v-bind="checkInAttrs" :disabled="!roomId" readonly/>
              </div>
              <span class="sb-error">{{ errors.checkIn }}</span>
            </div>
            <div class="sb-field">
              <label>Виїзд</label>
              <div class="calendar-input-wrapper" id="checkOut" ref="checkOut">
                <input ref="checkOutInput" @focus="activeField = 'checkOut'" :value="format(checkOutModelValue)"
                       v-bind="checkOutAttrs" :disabled="!roomId" readonly/>
              </div>
              <span class="sb-error">{{ errors.checkOut }}</span>
            </div>
          </div>
        </div>

        <div class="sb-divider"></div>

        <div class="sb-row">
          <div class="sb-field">
            <label>Імʼя гостя</label>
            <input v-model="name" v-bind="nameAttrs" type="text"/>
            <span class="sb-error">{{ errors.name }}</span>
          </div>
          <div class="sb-field">
            <label>Телефон</label>
            <input v-model="phone" v-bind="phoneAttrs" type="tel"/>
            <span class="sb-error">{{ errors.phone }}</span>
          </div>
        </div>

        <div class="sb-field" style="max-width: 50%">
          <label>Кількість гостей</label>
          <select v-model="guests" v-bind="guestsAttrs">
            <option disabled value="">Кількість гостей</option>
            <option
                v-for="n in (rooms[roomId]?.capacity ?? 1)"
                :key="n"
                :value="n"
            >
              {{ n }}
            </option>
          </select>
          <span class="sb-error">{{ errors.guests }}</span>
        </div>

        <div class="sb-divider"></div>

        <div class="sb-total">
          <span class="sb-total-label">До сплати</span>
          <span class="sb-total-amount">₴ {{ grandTotal }}</span>
        </div>

        <button class="sb-submit">Підтвердити бронювання</button>

      </div>
    </div>
  </form>
  <DateRangePicker
      v-model:activeField="activeField"
      v-model:checkIn="checkInModelValue"
      v-model:checkOut="checkOutModelValue"
      :check-in-target="inRef"
      :check-out-target="outRef"
      :dates="availability"
      :max-stay="MAX_STAY"
  />
</template>

<style scoped>
* {
  font-family: var(--font-description), sans-serif;
}

.calendar-input-wrapper {
  position: relative;
}

form {
  display: flex;
  flex-direction: column;
  gap: .5rem;
}

.error {
  color: red;
  font-size: .875rem;
}

.sb-wrap {
  font-family: 'Montserrat', sans-serif;
  padding: 2rem 1rem 1rem;
  max-width: 520px;
  margin: 0 auto;
}

.sb-card {
  background: #FFFBF5;
  border: 1.5px solid #F9C492;
  border-radius: 20px;
  overflow: hidden;
}

.sb-header {
  background: linear-gradient(135deg, #FB9558 0%, #D26B2E 100%);
  padding: 1.5rem 1.75rem 1.25rem;
  position: relative;
}

.sb-header-leaf {
  position: absolute;
  right: 1.5rem;
  top: 50%;
  transform: translateY(-50%);
  opacity: 0.18;
  font-size: 64px;
  line-height: 1;
  pointer-events: none;
  user-select: none;
}

.sb-header h2 {
  font-family: 'Kurale', serif;
  color: #fff;
  font-size: 1.4rem;
  margin: 0 0 .2rem;
  font-weight: 400;
  letter-spacing: .01em;
}

.sb-header p {
  color: rgba(255, 255, 255, 0.75);
  font-size: .78rem;
  margin: 0;
  letter-spacing: .04em;
  text-transform: uppercase;
}

.sb-body {
  padding: 1.5rem 1.75rem;
  display: flex;
  flex-direction: column;
  gap: 1.1rem;
}

.sb-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .75rem;
}

.sb-field {
  display: flex;
  flex-direction: column;
  gap: .35rem;
}

.sb-field label {
  font-size: .7rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: .08em;
  color: #D26B2E;
}

.sb-field input,
.sb-field select {
  font-family: 'Montserrat', sans-serif;
  font-size: .875rem;
  color: #202225;
  background: #fff;
  border: 1.5px solid #F9C492;
  border-radius: 10px;
  padding: .55rem .75rem;
  outline: none;
  transition: border-color .2s, box-shadow .2s;
  appearance: none;
  -webkit-appearance: none;
  width: 100%;
  box-sizing: border-box;

  &:disabled {
    opacity: 0.3;
    cursor: not-allowed;
  }
}

.sb-field select {
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' viewBox='0 0 12 8'%3E%3Cpath d='M1 1l5 5 5-5' stroke='%23D26B2E' stroke-width='1.8' fill='none' stroke-linecap='round'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right .75rem center;
  padding-right: 2rem;
}

.sb-field input:focus:not(:disabled),
.sb-field select:focus:not(:disabled) {
  border-color: #FB9558;
  box-shadow: 0 0 0 3px rgba(251, 149, 88, .15);
}

.sb-field input:hover:not(:disabled),
.sb-field select:hover:not(:disabled) {
  border-color: #FB9558;
}

.sb-error {
  font-size: .72rem;
  color: #dc3545;
  min-height: .9rem;
  letter-spacing: .01em;
}

.sb-divider {
  height: 1px;
  background: #FFF2D4;
  margin: .1rem 0;
}

.sb-total {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #FFF2D4;
  border-radius: 12px;
  padding: .85rem 1.1rem;
  border: 1px solid #F9C492;
}

.sb-total-label {
  font-size: .75rem;
  text-transform: uppercase;
  letter-spacing: .07em;
  color: #D26B2E;
  font-weight: 600;
}

.sb-total-amount {
  font-family: 'Kurale', serif;
  font-size: 1.35rem;
  color: #D26B2E;
  font-weight: 400;
}

.sb-submit {
  font-family: 'Montserrat', sans-serif;
  font-size: .875rem;
  font-weight: 600;
  letter-spacing: .06em;
  text-transform: uppercase;
  color: #fff;
  background: linear-gradient(135deg, #FB9558 0%, #D26B2E 100%);
  border: none;
  border-radius: 12px;
  padding: .85rem 1.5rem;
  cursor: pointer;
  transition: opacity .2s, transform .15s;
  width: 100%;
}

.sb-submit:hover {
  opacity: .9;
  transform: translateY(-1px);
}

.sb-submit:active {
  opacity: 1;
  transform: translateY(0);
}

.sb-dates-label {
  font-size: .7rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: .08em;
  color: #D26B2E;
  margin-bottom: -.4rem;
}

.sb-accent-dot {
  display: inline-block;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #99B738;
  margin-right: 6px;
  vertical-align: middle;
  position: relative;
  top: -1px;
}

</style>

