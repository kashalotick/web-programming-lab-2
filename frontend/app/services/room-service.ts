import type {DateRangeQueryParams} from "~/types/common";
const config = useRuntimeConfig()

export const RoomAPI = {
  async getAll() {
    const result = await $fetch('/rooms', {
      baseURL: config.public.apiBase
    });
    return result;
  },
  async get(id: number) {
    const result = await $fetch(`/rooms/${id}`,{
      baseURL: config.public.apiBase
    });
    return result;
  },

  async getReservations(id: number) {
    const result = await $fetch(`/rooms/${id}/reservations`,{
      baseURL: config.public.apiBase
    });
    return result;
  },
  async getAvailability(id: number, params: DateRangeQueryParams) {
    const result = await $fetch(`/rooms/${id}/availability`, {
      query: params,
      baseURL: config.public.apiBase
    });
    return result;
  }
}