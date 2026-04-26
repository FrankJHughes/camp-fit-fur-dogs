import { describe, it, expect } from 'vitest';
import { validateDogForm } from '../../../src/lib/dogs/validateDogForm';

describe('validateDogForm', () => {
  it('returns no errors when all fields are valid', () => {
    const result = validateDogForm({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
    expect(result).toEqual({});
  });

  it('returns an error when name is empty', () => {
    const result = validateDogForm({
      name: '',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
    expect(result).toEqual({ name: "Please enter your dog's name" });
  });

  it('returns an error when name is only whitespace', () => {
    const result = validateDogForm({
      name: '   ',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
    expect(result).toEqual({ name: "Please enter your dog's name" });
  });

  it('returns an error when breed is empty', () => {
    const result = validateDogForm({
      name: 'Buddy',
      breed: '',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });
    expect(result).toEqual({ breed: 'Please enter a breed' });
  });

  it('returns an error when dateOfBirth is empty', () => {
    const result = validateDogForm({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '',
      sex: 'Male',
    });
    expect(result).toEqual({ dateOfBirth: 'Please enter a date of birth' });
  });

  it('returns an error when sex is empty', () => {
    const result = validateDogForm({
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: '',
    });
    expect(result).toEqual({ sex: 'Please select a sex' });
  });

  it('returns errors for all fields when all are empty', () => {
    const result = validateDogForm({
      name: '',
      breed: '',
      dateOfBirth: '',
      sex: '',
    });
    expect(result).toEqual({
      name: "Please enter your dog's name",
      breed: 'Please enter a breed',
      dateOfBirth: 'Please enter a date of birth',
      sex: 'Please select a sex',
    });
  });
});
