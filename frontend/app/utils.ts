const MONTHS_UA = [
  'січ', 'лют', 'бер', 'кві', 'тра', 'чер',
  'лип', 'сер', 'вер', 'жов', 'лис', 'гру'
];

// "12 лип"
export function formatShortDate(date: Date) {
  const d = new Date(date);
  return `${d.getDate()} ${MONTHS_UA[d.getMonth()]}`;
}

// "10 лип 2025, 14:32"
export function formatFullDate(date: Date) {
  const d = new Date(date);
  const day = d.getDate();
  const month = MONTHS_UA[d.getMonth()];
  const year = d.getFullYear();
  const hours = String(d.getHours()).padStart(2, '0');
  const mins = String(d.getMinutes()).padStart(2, '0');
  return `${day} ${month} ${year}, ${hours}:${mins}`;
}