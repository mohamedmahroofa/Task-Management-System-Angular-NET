export interface UserManagement {
    userId: number,
    name: string,
    dateCreated: Date | string;
    isDeleted: boolean;
    color:string;
}