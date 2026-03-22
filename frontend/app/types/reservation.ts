export interface Reservation {
    id: number;
    roomId: number;
    guestId: number;
    guestCount: number;
    checkIn: Date;
    checkOut: Date;
    createdAt: Date;
    isActive: boolean;
    grandTotal: number;
}


export interface ReservationDtoGet {
    id: number;
    roomId: number;
    guestId: number;
    guestCount: number;
    checkIn: string;
    checkOut: string;
    createdAt: string;
    isActive: boolean;
    grandTotal: number;
}

export interface ReservationDtoUpdate {
    // TODO: make
}
export interface ReservationDtoCreate {
    reservation: {
        roomId: number;
        guestCount: number;
        checkIn: string;
        checkOut: string;
    }
    guest: {
        name: string,
        email: string,
    }
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


