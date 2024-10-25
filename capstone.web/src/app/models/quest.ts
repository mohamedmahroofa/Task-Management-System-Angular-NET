export interface Quest {
    QuestId: number,
    Name: string,
    dateCreated: Date | string;
    dueDate: Date | string;
    isDeleted: boolean;
    priorityId: number,
    categoryId: number
    
}