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
  from?: string | null;
  to?: string | null;
  roomId?: number | null;
  isActive?: boolean| null;
  sortBy?: 'check_in' | 'created_at' | 'grand_total'| null;
  isDescending?: boolean;
}