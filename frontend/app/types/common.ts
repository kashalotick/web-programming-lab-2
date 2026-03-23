export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: unknown;
}


export interface DateRangeQueryParams {
  from?: string;
  to?: string;
}

export interface ReservationsQueryParams {
  from?: string;
  to?: string;
  roomId?: number;
  isActive?: boolean;
  sortBy?: string;
  isDescending?: boolean;
}