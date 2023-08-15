export interface ICreateCategoryDto {    
    name: string;
    description: string;     
}  

export interface ICategoryDto{
    id: string;
    name: string;
    description: string;     
}
  
export interface IPagedResultDtoOfCategoryDto {
    totalCount: number | undefined;
    items: ICategoryDto[] | undefined;
}
  
export interface IResult {
    result: IPagedResultDtoOfCategoryDto;
}
  
export interface IResultObject {
    result: ICategoryDto;
}

export class CategoryDto implements ICategoryDto{
    id: string;
    name: string;
    description: string;
}

export class CreateCategoryDto implements ICreateCategoryDto{
    name: string;
    description: string;
}




  
  
  