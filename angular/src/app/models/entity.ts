export interface EntityDto<T> {
  items?: EntityDto<T>[] | undefined;
  totalCount?: number | undefined;
}

export interface ResultDto<T>{
  result : T | undefined;
  totalCount?: number | undefined;
}