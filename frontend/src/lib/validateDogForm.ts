import type { DogFormValues } from '../components/dogs/DogForm';

export function validateDogForm(values: DogFormValues): Record<string, string> {
  const errors: Record<string, string> = {};
  if (!values.name.trim()) errors.name = "Please enter your dog's name";
  if (!values.breed.trim()) errors.breed = 'Please enter a breed';
  if (!values.dateOfBirth.trim()) errors.dateOfBirth = 'Please enter a date of birth';
  if (!values.sex) errors.sex = 'Please select a sex';
  return errors;
}