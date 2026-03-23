const config = useRuntimeConfig()

export const GuestAPI = {
  async getAll() {
    const result = await $fetch('/guests', {
      baseURL: config.public.apiBase
    });
    console.log(result);
    return result;
  },
  async get(id: number) {
    const result = await $fetch(`/guests/${id}`,{
      baseURL: config.public.apiBase
    });
    console.log(result);
    return result;
  },

  async getReservations(id: number) {
    const result = await $fetch(`/guests/${id}/reservations`,{
      baseURL: config.public.apiBase
    });
    console.log(result)
    return result;
  },
}