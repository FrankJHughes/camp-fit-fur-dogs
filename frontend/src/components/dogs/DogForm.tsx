// src/components/dogs/DogForm.tsx
'use client';

import { FormField } from '@/lib/components/FormField';
import { FieldError } from '@/lib/components/FieldError';
import { useFormStateMachine } from '@/lib/forms/useFormStateMachine';
import type { FormCommand } from '@/lib/forms/formCommand';
import type { DogFormValues } from '@/lib/dogs/dogModel';
import {
  dogFormDefaultValues,
  dogFormLabels,
} from '@/lib/dogs/dogModel';
import { validateDogForm } from '@/lib/dogs/validateDogForm';

interface DogFormProps {
  title: string;
  submitLabel: string;
  command: FormCommand<DogFormValues>;
  initialValues?: DogFormValues;
}

export function DogForm({
  title,
  submitLabel,
  command,
  initialValues = dogFormDefaultValues,
}: DogFormProps) {
  const externalErrors = {
    ...(command.errors ?? {}),
    ...(command.error ? { form: command.error } : {}),
  };

  const {
    values,
    displayErrors,
    update,
    handleSubmit,
    isSubmitting: internalSubmitting,
  } = useFormStateMachine<DogFormValues>({
    initialValues,
    externalErrors,
    onSubmit: async (vals: DogFormValues) => {
      const validation = await validateDogForm(vals);
      if (validation && Object.keys(validation).length > 0) {
        return validation;
      }

      try {
        await command.run(vals);

        if (command.errors && Object.keys(command.errors).length > 0) {
          return command.errors;
        }

        if (command.error) {
          return { form: command.error };
        }

        return;
      } catch (err) {
        // eslint-disable-next-line no-console
        console.error(err);
        return { form: 'A network error occurred. Please try again.' };
      }
    },
  });

  const isSubmitting = internalSubmitting || command.isSubmitting;

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>{title}</h1>

      <FieldError id="error-form" message={displayErrors.form} />

      <FormField
        label={dogFormLabels.name}
        name="name"
        error={displayErrors.name}
      >
        {(fieldProps) => {
          const { id: fpId, ...rest } = fieldProps;
          const id = fpId ?? 'field-name';
          return (
            <input
              id={id}
              type="text"
              value={values.name}
              onChange={update('name')}
              {...rest}
              disabled={isSubmitting}
            />
          );
        }}
      </FormField>

      <FormField
        label={dogFormLabels.breed}
        name="breed"
        error={displayErrors.breed}
      >
        {(fieldProps) => {
          const { id: fpId, ...rest } = fieldProps;
          const id = fpId ?? 'field-breed';
          return (
            <input
              id={id}
              type="text"
              value={values.breed}
              onChange={update('breed')}
              {...rest}
              disabled={isSubmitting}
            />
          );
        }}
      </FormField>

      <FormField
        label={dogFormLabels.dateOfBirth}
        name="dateOfBirth"
        error={displayErrors.dateOfBirth}
      >
        {(fieldProps) => {
          const { id: fpId, ...rest } = fieldProps;
          const id = fpId ?? 'field-dateOfBirth';
          return (
            <input
              id={id}
              type="date"
              value={values.dateOfBirth}
              onChange={update('dateOfBirth')}
              {...rest}
              disabled={isSubmitting}
            />
          );
        }}
      </FormField>

      <FormField
        label={dogFormLabels.sex}
        name="sex"
        error={displayErrors.sex}
      >
        {(fieldProps) => {
          const { id: fpId, ...rest } = fieldProps;
          const id = fpId ?? 'field-sex';
          return (
            <select
              id={id}
              value={values.sex}
              onChange={(e) => update('sex')(e.target.value as any)}
              {...rest}
              disabled={isSubmitting}
            >
              <option value="">Select</option>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
            </select>
          );
        }}
      </FormField>

      <button type="submit" disabled={isSubmitting}>
        {submitLabel}
      </button>
    </form>
  );
}

export default DogForm;
