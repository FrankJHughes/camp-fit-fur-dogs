//
// FORM VALUES
//
export interface DogFormValues {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: 'Male' | 'Female';
}

export const dogFormDefaultValues: DogFormValues = {
  name: '',
  breed: '',
  dateOfBirth: '',
  sex: 'Male', // default must match schema
};

export const dogFormLabels: Record<keyof DogFormValues, string> = {
  name: 'Name',
  breed: 'Breed',
  dateOfBirth: 'Date of Birth',
  sex: 'Sex',
};

//
// COMMANDS
//
export interface RegisterDogCommand {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: 'Male' | 'Female';
}

export interface EditDogProfileCommand {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: 'Male' | 'Female';
}

export function mapDogFormValuesToRegisterCommand(
  values: DogFormValues
): RegisterDogCommand {
  return { ...values };
}

export function mapDogFormValuesToEditCommand(
  values: DogFormValues
): EditDogProfileCommand {
  return { ...values };
}

//
// QUERY TYPES
//
export interface DogProfile {
  id: string;
  ownerId: string;
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: 'Male' | 'Female';
}

export interface DogListItem {
  id: string;
  name: string;
  breed: string;
}

export interface ListDogsByCurrentUserResponse {
  dogs: DogListItem[];
}
