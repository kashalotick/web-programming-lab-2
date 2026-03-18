export interface Reservation {
  id: number;
  roomId: number;
  checkIn: Date;
  checkOut: Date;
  createdAt: Date;
  isActive: boolean;
  grandTotal: number;
}

export interface ReservationDtoGet {
  id: number;
  roomId: number;
  checkIn: string;
  checkOut: string;
  createdAt: string;
  isActive: boolean;
  grandTotal: number;
}

export function restore(dto: ReservationDtoGet): Reservation {
  return {
    id: dto.id,
    roomId: dto.roomId,
    checkIn: new Date(dto.checkIn),
    checkOut: new Date(dto.checkOut),
    createdAt: new Date(dto.createdAt),
    isActive: dto.isActive,
    grandTotal: dto.grandTotal,
  }
}