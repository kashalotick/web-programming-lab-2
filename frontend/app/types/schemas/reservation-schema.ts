import { z } from 'zod'

// Схеми
export const guestCreateSchema = z.object({
  name: z.string().min(1, 'Ім\'я обов\'язкове'),
  email: z.string().email('Невалідний email'),
})

export const reservationCreateSchema = z.object({
  roomId: z.number().int().positive('ID кімнати обов\'язковий'),
  guestCount: z.number().int().min(1, 'Мінімум 1 гість'),
  checkIn: z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Формат: YYYY-MM-DD'),
  checkOut: z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Формат: YYYY-MM-DD'),
  grandTotal: z.number().nullable().optional(),
}).refine(
  (data) => new Date(data.checkOut) > new Date(data.checkIn),
  { message: 'Дата виїзду має бути пізніше дати заїзду', path: ['checkOut'] }
)

export const reservationUpdateSchema = z.object({
  roomId: z.number().int().positive().nullable().optional(),
  guestCount: z.number().int().min(1).nullable().optional(),
  checkIn: z.string().regex(/^\d{4}-\d{2}-\d{2}$/).nullable().optional(),
  checkOut: z.string().regex(/^\d{4}-\d{2}-\d{2}$/).nullable().optional(),
  grandTotal: z.number().nullable().optional(),
}).refine(
  (data) => {
    if (data.checkIn && data.checkOut) {
      return new Date(data.checkOut) > new Date(data.checkIn)
    }
    return true
  },
  { message: 'Дата виїзду має бути пізніше дати заїзду', path: ['checkOut'] }
)