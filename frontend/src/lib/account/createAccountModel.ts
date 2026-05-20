// src/lib/account/createAccountModel.ts
import { CreateAccountSchema, type CreateAccountValues } from './createAccountSchema';
import type { CreateAccountCommand } from '@/api/account/types'; // <- updated import

export const createAccountDefaultValues: CreateAccountValues = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
};

export const createAccountFieldOrder: Array<keyof CreateAccountValues> = [
  'firstName',
  'lastName',
  'email',
  'phone',
  'password',
  'confirmPassword',
];

export const createAccountLabels: Record<keyof CreateAccountValues, string> = {
  firstName: 'First Name',
  lastName: 'Last Name',
  email: 'Email',
  phone: 'Phone',
  password: 'Password',
  confirmPassword: 'Confirm Password',
};

export const createAccountAutoComplete: Partial<
  Record<keyof CreateAccountValues, string>
> = {
  firstName: 'given-name',
  lastName: 'family-name',
  email: 'email',
  phone: 'tel',
  password: 'new-password',
  confirmPassword: 'new-password',
};

export const createAccountSchema = CreateAccountSchema;

export function mapCreateAccountValuesToCommand(
  values: CreateAccountValues
): CreateAccountCommand {
  const { confirmPassword, ...rest } = values;
  return rest;
}
