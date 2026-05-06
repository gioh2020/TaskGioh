export type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  assignedUserId: string;
  assignedUserName: string;
  assignedUserEmail: string;
  createdAt: string;
  additionalInfo?: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  assignedUserId: string;
  additionalInfo?: string;
}

export interface ChangeStatusRequest {
  newStatus: string;
}

export interface UpdateAdditionalInfoRequest {
  estimatedEndDate: string | null;
}
