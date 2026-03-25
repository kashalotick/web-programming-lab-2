import type {ReservationsQueryParams} from "~/types/common";
import type {ReservationDtoCreate, ReservationDtoUpdate} from "~/types/reservation";
import type {GuestDtoCreate} from "~/types/guest";

const config = useRuntimeConfig()

export const ReservationAPI = {
  async getAll(params: ReservationsQueryParams) {
    const result = await $fetch('/reservations', {
      baseURL: config.public.apiBase,
      query: params
    });
    return result;
  },
  async get(id: number) {
    const result = await $fetch(`/reservations/${id}`,{
      baseURL: config.public.apiBase
    });
    return result;
  },
  async getFull(id: number) {
    const result = await $fetch(`/reservations/${id}/full`,{
      baseURL: config.public.apiBase
    });
    return result;
  },

  async delete(id: number) {
    const result = await $fetch(`/reservations/${id}`,{
      method: 'DELETE',
      baseURL: config.public.apiBase,
      headers: {
        'Administrator': 'true'
      }
    });
    return result;
  },
  async cancel(id: number) {
    const result = await $fetch(`/reservations/${id}/cancel`,{
      method: 'POST',
      baseURL: config.public.apiBase,
      headers: {
        'Administrator': 'true'
      }
    });
    return result;
  },

  async makeReservation(reservation: ReservationDtoCreate, guest: GuestDtoCreate) {
    const body =  {
      reservation: reservation,
      guest: guest
    }
    const result = await $fetch(`/reservations/reserve-room`,{
      method: 'POST',
      baseURL: config.public.apiBase,
      body: body
    });
    return result;
  },
  async create(reservation: ReservationDtoCreate, guest: GuestDtoCreate) {
    const body =  {
      reservation: reservation,
      guest: guest
    }
    const result = await $fetch(`/reservations`,{
      method: 'POST',
      baseURL: config.public.apiBase,
      headers: {
        'Administrator': 'true'
      },
      body: body
    });
    return result;
  },
  async update(id: number, reservation: ReservationDtoUpdate) {
    const result = await $fetch(`/reservations/${id}`,{
      method: 'PUT',
      baseURL: config.public.apiBase,
      headers: {
        'Administrator': 'true'
      },
      body: reservation
    });
    return result;
  }
}