import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { FormField } from '../../../src/lib/components/FormField';

describe('FormField', () => {
  it('renders the label text', () => {
    render(
      <FormField label="Name" name="name" error={undefined}>
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    expect(screen.getByLabelText('Name')).toBeInTheDocument();
  });

  it('passes aria-invalid via fieldProps when error exists', () => {
    render(
      <FormField label="Name" name="name" error="Please enter your dog's name">
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    expect(screen.getByLabelText('Name')).toHaveAttribute('aria-invalid', 'true');
  });

  it('does not set aria-invalid when there is no error', () => {
    render(
      <FormField label="Name" name="name" error={undefined}>
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    expect(screen.getByLabelText('Name')).not.toHaveAttribute('aria-invalid');
  });

  it('passes aria-describedby matching the error element id', () => {
    render(
      <FormField label="Name" name="name" error="Please enter your dog's name">
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    const input = screen.getByLabelText('Name');
    const errorId = input.getAttribute('aria-describedby');
    expect(errorId).toBe('error-name');
    expect(document.getElementById(errorId!)).toHaveTextContent(
      "Please enter your dog's name",
    );
  });

  it('renders the error with role="alert" when error exists', () => {
    render(
      <FormField label="Name" name="name" error="Please enter your dog's name">
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    expect(screen.getByRole('alert')).toHaveTextContent(
      "Please enter your dog's name",
    );
  });

  it('does not render an alert when there is no error', () => {
    render(
      <FormField label="Name" name="name" error={undefined}>
        {(fieldProps) => <input {...fieldProps} />}
      </FormField>,
    );
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('works with select elements', () => {
    render(
      <FormField label="Sex" name="sex" error="Please select a sex">
        {(fieldProps) => (
          <select {...fieldProps}>
            <option value="">Select</option>
            <option value="Male">Male</option>
          </select>
        )}
      </FormField>,
    );
    expect(screen.getByLabelText('Sex')).toHaveAttribute('aria-invalid', 'true');
    expect(screen.getByRole('alert')).toHaveTextContent('Please select a sex');
  });
});
