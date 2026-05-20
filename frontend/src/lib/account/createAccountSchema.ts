import { z } from 'zod';

/**
 * Create Account Schema
 *
 * This is the single source of truth for all Create Account validation.
 * Client-side validation, server-side validation, and integration tests
 * should all rely on this schema.
 */
export const CreateAccountSchema = z
  .object({
    firstName: z
      .string()
      .trim()
      .min(1, { message: 'First name is required' })
      .max(100, { message: 'First name must be at most 100 characters' })
      .regex(/^[A-Za-z' -]+$/, {
        message: 'First name contains invalid characters.',
      }),

    lastName: z
      .string()
      .trim()
      .min(1, { message: 'Last name is required' })
      .max(100, { message: 'Last name must be at most 100 characters' })
      .regex(/^[A-Za-z' -]+$/, {
        message: 'Last name contains invalid characters.',
      }),

    email: z
      .string()
      .trim()
      .min(1, { message: 'Email is required' })
      .email({ message: 'Invalid email address' })
      .regex(
        /^(?!\.)[A-Za-z0-9._%+-]+@(?!-)([A-Za-z0-9-]+\.)+[A-Za-z]{2,63}$/,
        { message: 'Email format is invalid.' }
      ),

    phone: z
      .string()
      .trim()
      .regex(/^[0-9+\-\s().]+$/, {
        message: 'Phone number contains invalid characters.',
      })
      .refine((val) => val.replace(/\D/g, '').length >= 10, {
        message: 'Phone number must contain at least 10 digits.',
      }),

    password: z
      .string()
      .min(1, { message: 'Password is required' })
      .min(8, { message: 'Password must be at least 8 characters' })
      .regex(/[A-Za-z]/, {
        message: 'Password must contain at least one letter.',
      })
      .regex(/[0-9]/, {
        message: 'Password must contain at least one number.',
      }),

    confirmPassword: z
      .string()
      .min(1, { message: 'Confirm password is required' }),
  })
  .superRefine((data, ctx) => {
    if (data.password !== data.confirmPassword) {
      ctx.addIssue({
        code: 'custom',
        path: ['confirmPassword'],
        message: 'Passwords do not match',
      });
    }
  });

export type CreateAccountValues = z.infer<typeof CreateAccountSchema>;
