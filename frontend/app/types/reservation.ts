import type {GuestDtoGet} from "~/types/guest";
import type {RoomDtoGet} from "~/types/room";
export interface Reservation {
    id: number;
    roomId: number;
    guestId: number;
    guestCount: number;
    checkIn: Date;
    checkOut: Date;
    createdAt: Date;
    grandTotal: number;
    isActive: boolean;
}
export interface ReservationDtoCreate {
    roomId: number;
    guestCount: number;
    checkIn: string; // date (ISO 8601: "YYYY-MM-DD")
    checkOut: string; // date (ISO 8601: "YYYY-MM-DD")
    grandTotal?: number | null;
}

export interface ReservationDtoUpdate {
    roomId?: number | null;
    guestCount?: number | null;
    checkIn?: string | null;
    checkOut?: string | null;
    grandTotal?: number | null;
}

export interface ReservationDtoGet {
    id: number;
    roomId: number;
    guestId: number;
    guestCount: number;
    checkIn: string;
    checkOut: string;
    createdAt: string; // date-time
    grandTotal: number;
    isActive: boolean;
}
export interface ReservationFullDtoGet {
    id: number;
    room: RoomDtoGet;
    guest: GuestDtoGet;
    guestCount: number;
    checkIn: string;
    checkOut: string;
    createdAt: string; // date-time
    grandTotal: number;
    isActive: boolean;
}


export function restore(dto: ReservationDtoGet): Reservation {
    return {
        id: dto.id,
        roomId: dto.roomId,
        guestId: dto.guestId,
        guestCount: dto.guestCount,
        checkIn: new Date(dto.checkIn),
        checkOut: new Date(dto.checkOut),
        createdAt: new Date(dto.createdAt),
        isActive: dto.isActive,
        grandTotal: dto.grandTotal,
    }
}


