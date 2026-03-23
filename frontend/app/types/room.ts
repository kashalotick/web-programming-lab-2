export interface RoomDtoGet {
  id: number;
  name?: string | null;
  capacity: number;
  price: number;
  description?: string | null;
  imageUrl?: string | null;
}

export type AvailabilityDtoGet = Record<string, {
  isAvailable: boolean;
  price: number;
}>

