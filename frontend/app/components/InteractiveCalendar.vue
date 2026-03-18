<script setup lang="ts">
import {ref, computed} from 'vue'

const props = defineProps<{
  dates: Record<string, {
    available: boolean;
    price: number;
  }>,
  dateMin?: Date | null, // YYYY-MM-DD
  dateMax?: Date | null, // YYYY-MM-DD
  allowDisabled?: boolean,
  modelValue: string | null,
}>()

const emit = defineEmits(['update:modelValue', 'reset', 'ok'])

// --- Стан навігації ---
const today = new Date()
today.setHours(0, 0, 0, 0)

const currentYear = ref(today.getFullYear())
const currentMonth = ref(today.getMonth()) // 0-based

const MONTHS_UA = [
  'Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень',
  'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень',
]
const weekdays = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Нд']

const currentMonthLabel = computed(() => MONTHS_UA[currentMonth.value])

// --- Обчислення сітки ---
const daysInMonth = computed(() =>
    new Date(currentYear.value, currentMonth.value + 1, 0).getDate()
)

// Перший день місяця: 0=Нд → перетворюємо на Пн-based (0=Пн)
const firstDayOffset = computed(() => {
  const d = new Date(currentYear.value, currentMonth.value, 1).getDay()
  return (d + 6) % 7 // Нд(0)->6, Пн(1)->0, ...
})

// --- Навігація ---
const isPrevMonthDisabled = computed(() => {
  return (
      currentYear.value < today.getFullYear() ||
      (currentYear.value === today.getFullYear() && currentMonth.value <= today.getMonth())
  )
})

function prevMonth() {
  if (isPrevMonthDisabled.value) return
  if (currentMonth.value === 0) {
    currentMonth.value = 11
    currentYear.value--
  } else {
    currentMonth.value--
  }
}

function nextMonth() {
  if (currentMonth.value === 11) {
    currentMonth.value = 0
    currentYear.value++
  } else {
    currentMonth.value++
  }
}

// --- Хелпери ---
function dateKey(day: number) {
  const mm = String(currentMonth.value + 1).padStart(2, '0')
  const dd = String(day).padStart(2, '0')
  return `${currentYear.value}-${mm}-${dd}`
}

function getDayData(day: number) {
  return props.dates[dateKey(day)] ?? null
}

function isDayDisabled(day: number) {
  const date = new Date(currentYear.value, currentMonth.value, day)
  date.setHours(0, 0, 0, 0)

  const data = getDayData(day)

  if (props.dateMin) {
    if (date < props.dateMin) return true
  }
  if (props.dateMax) {
    if (date > props.dateMax) return true
  }
  if (!data && !props.allowDisabled) return true
  if (!data?.available && !props.allowDisabled) return true


  return false;
}

function isDayAvailable(day: number) {
  const data = getDayData(day)
  return data?.available
}

function getDayClasses(day: number) {
  const key = dateKey(day)
  const data = getDayData(day)
  const disabled = isDayDisabled(day)
  const d = new Date(currentYear.value, currentMonth.value, day)
  d.setHours(0, 0, 0, 0)

  return {
    'cal__cell--today': d.getTime() == today.getTime(),
    'cal__cell--disabled': disabled,
    'cal__cell--available': !disabled && data?.available,
    'cal__cell--no-data': !data,
    'cal__cell--selected': props.modelValue === key,
  }
}

// --- Вибір дати ---
function selectDay(day: number) {
  if (isDayDisabled(day)) return
  emit('update:modelValue', dateKey(day))
}

function resetDate() {
  emit('reset')
}
function okDate() {
  emit('ok')
}
</script>

<template>
  <div class="cal-wrap">
    <div class="cal">
      <div class="cal__header">
        <button class="cal__nav" @click="prevMonth" :disabled="isPrevMonthDisabled">&#8249;</button>
        <span class="cal__title">{{ currentMonthLabel }} {{ currentYear }}</span>
        <button class="cal__nav" @click="nextMonth">&#8250;</button>
      </div>

      <div class="cal__weekdays">
        <span v-for="day in weekdays" :key="day" class="cal__weekday">{{ day }}</span>
      </div>

      <div class="cal__grid">
        <span
            v-for="n in firstDayOffset" :key="'e'+n"
            class="cal__cell cal__cell--empty"
        />
        <button
            v-for="day in daysInMonth" :key="day"
            class="cal__cell"
            :class="getDayClasses(day)"
            :disabled="isDayDisabled(day)"
            @click.stop="selectDay(day)"
        >
          <span class="cal__day-num">{{ day }}</span>
          <span v-if="getDayData(day)?.price && isDayAvailable(day) && !isDayDisabled(day)" class="cal__day-price">
            {{ getDayData(day)?.price }}₴
          </span>
        </button>
      </div>

      <div class="cal__footer">
        <button class="cal__footer-button style-2" @click="resetDate">Скинути</button>
        <button class="cal__footer-button style-1" @click="okDate">ОК</button>
      </div>
    </div>
  </div>
</template>
<style scoped>
.cal-wrap {
  max-width: 420px;
  font-family: var(--font-description), sans-serif;
  flex: 1;
}

.cal {
  background: var(--background);
  border: 1.5px solid var(--accent-light);
  border-radius: 18px;
  overflow: hidden;
}

.cal__header {
  background: linear-gradient(135deg, var(--accent) 0%, var(--accent-dark) 100%);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: .85rem 1.1rem;
}

.cal__title {
  font-family: var(--font-heading), serif;
  font-size: 1.125rem;
  color: var(--white);
  font-weight: 400;
  letter-spacing: .02em;
}

.cal__nav {
  background: rgba(255, 255, 255, .15);
  border: 1px solid rgba(255, 255, 255, .25);
  border-radius: 8px;
  color: var(--white);
  width: 30px;
  height: 30px;
  font-size: 1.1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background .15s;
  line-height: 1;
  padding: 0;
}

.cal__nav:hover:not(:disabled) {
  background: rgba(255, 255, 255, .28);
}

.cal__nav:disabled {
  opacity: .35;
  cursor: default;
}

.cal__weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background: var(--background-dark);
  border-bottom: 1px solid var(--accent-light);
  padding: .5rem .5rem;
}

.cal__weekday {
  font-family: var(--font-description), sans-serif;
  font-size: .825rem;
  font-weight: 600;
  letter-spacing: .1em;
  color: var(--accent-dark);
  text-align: center;
  padding: .2rem 0;
}

.cal__grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 3px;
  padding: .6rem .5rem .5rem;
}


.cal__cell {
  font-family: var(--font-description), sans-serif;
  background: var(--white);
  border: 1px solid var(--accent-light);
  border-radius: 9px;
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1px;
  cursor: pointer;
  padding: 0;
  position: relative;
  transition: border-color .15s, background .15s, transform .1s;

  &:hover:not(:disabled):not(.cal__cell--empty) {
    border-color: var(--accent);
    background: var(--background-dark);
    transform: scale(1.05);
    z-index: 1;
  }

  &:active:not(:disabled) {
    transform: scale(.97);
  }

  &:disabled {
    background: var(--background);
    border-color: var(--gray);
    cursor: default;
    opacity: .55;
  }
}

.cal__cell--empty {
  background: transparent;
  border: none;
  cursor: default;
}


.cal__cell--today {
  border-color: var(--accent-dark);
  border-width: 2px;

  &:disabled {
    border-color: var(--accent-light);
    border-width: 2px;
  }

  .cal__day-num {
    color: var(--accent-dark);
    font-weight: 700;
  }
}

.cal__cell--selected {
  background: var(--accent);

  &:hover {
    background: var(--accent-dark) !important;
  }

  .cal__day-num {
    color: var(--white);
  }

  .cal__day-price {
    color: rgba(255, 255, 255, .8);
  }
}


.cal__cell--in-range {
  background: var(--background-dark);
  border-color: var(--accent-light);
  border-radius: 0;
}

.cal__day-num {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text);
  line-height: 1;
}

.cal__day-price {
  font-size: .875rem;
  color: var(--accent-dark);
  letter-spacing: .02em;
  line-height: 1;
}

.cal__footer {
  display: flex;
  justify-content: flex-end;
  gap: .5rem;
  padding: .75rem .75rem;
  border-top: 1px solid var(--accent-light);
  margin-top: .125rem;
}
.cal__footer-button {
  width: fit-content;
  padding: .5rem 1.2rem;
}

</style>