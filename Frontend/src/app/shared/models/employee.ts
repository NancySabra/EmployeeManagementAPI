export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  hireDate: string;
  salary: number;
  departmentId: number;
  departmentName?: string;
  isActive?: boolean;
  createdDate?: string;
  createdBy?: number;
  updatedBy?: number;
  updatedDate?: string;
}