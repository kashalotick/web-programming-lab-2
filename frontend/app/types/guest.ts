export interface GuestDtoCreate {
  name: string;
  email: string;
}

export interface GuestDtoGet {
  id: number;
  name?: string | null;
  email?: string | null;
}
