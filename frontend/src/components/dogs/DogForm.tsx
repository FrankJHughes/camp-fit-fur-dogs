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
  // Merge form-level error into the external errors object under the "form" key
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
      // Run local validation first; if there are validation errors, abort submit
      const validation = await validateDogForm(vals);
      if (validation && Object.keys(validation).length > 0) {
        return validation;
      }

      // Invoke the provided command using the canonical run method.
      // The command implementation is expected to set its own errors/error state.
      try {
        await command.run(vals);

        // If the command produced field errors, return them so the state machine
        // can display them immediately.
        if (command.errors && Object.keys(command.errors).length > 0) {
          return command.errors;
        }

        // If the command produced a form-level error, return it under "form"
        if (command.error) {
          return { form: command.error };
        }

        // No errors -> success (return void)
        return;
      } catch (err) {
        // Log the error for debugging and return a generic form-level error
        // eslint-disable-next-line no-console
        console.error(err);
        return { form: 'A network error occurred. Please try again.' };
      }
    },
  });

  // Consider both internal and external submitting states
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
        {(fieldProps) => (
          <input
            id="field-name"
            type="text"
            value={values.name}
            onChange={update('name')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.breed}
        name="breed"
        error={displayErrors.breed}
      >
        {(fieldProps) => (
          <input
            id="field-breed"
            type="text"
            value={values.breed}
            onChange={update('breed')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.dateOfBirth}
        name="dateOfBirth"
        error={displayErrors.dateOfBirth}
      >
        {(fieldProps) => (
          <input
            id="field-dateOfBirth"
            type="date"
            value={values.dateOfBirth}
            onChange={update('dateOfBirth')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.sex}
        name="sex"
        error={displayErrors.sex}
      >
        {(fieldProps) => (
          <select
            id="field-sex"
            value={values.sex}
            onChange={(e) => update('sex')(e.target.value as any)}
            {...fieldProps}
            disabled={isSubmitting}
          >
            <option value="">Select</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        )}
      </FormField>

      <button type="submit" disabled={isSubmitting}>
        {submitLabel}
      </button>
    </form>
  );
}

export default DogForm;
