import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type { UseFormRegister, FieldErrors } from "react-hook-form";
import type { SessionFormValues } from "../schemas/sessionFormSchema";

interface ZoneInputsProps {
  register: UseFormRegister<SessionFormValues>;
  errors: FieldErrors<SessionFormValues>;
}

export function ZoneInputs({ register, errors }: ZoneInputsProps) {
  return (
    <div className="space-y-4">
      <Field>
        <FieldLabel htmlFor="paintMakes">Paint Makes</FieldLabel>
        <Input
          id="paintMakes"
          type="number"
          {...register("paintMakes", { valueAsNumber: true })}
        />
        {errors.paintMakes && (
          <span className="text-sm text-destructive">{errors.paintMakes.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="paintAttempts">Paint Attempts</FieldLabel>
        <Input
          id="paintAttempts"
          type="number"
          {...register("paintAttempts", { valueAsNumber: true })}
        />
        {errors.paintAttempts && (
          <span className="text-sm text-destructive">{errors.paintAttempts.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="midrangeMakes">Midrange Makes</FieldLabel>
        <Input
          id="midrangeMakes"
          type="number"
          {...register("midrangeMakes", { valueAsNumber: true })}
        />
        {errors.midrangeMakes && (
          <span className="text-sm text-destructive">{errors.midrangeMakes.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="midrangeAttempts">Midrange Attempts</FieldLabel>
        <Input
          id="midrangeAttempts"
          type="number"
          {...register("midrangeAttempts", { valueAsNumber: true })}
        />
        {errors.midrangeAttempts && (
          <span className="text-sm text-destructive">{errors.midrangeAttempts.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="threePointMakes">Three Point Makes</FieldLabel>
        <Input
          id="threePointMakes"
          type="number"
          {...register("threePointMakes", { valueAsNumber: true })}
        />
        {errors.threePointMakes && (
          <span className="text-sm text-destructive">{errors.threePointMakes.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="threePointAttempts">Three Point Attempts</FieldLabel>
        <Input
          id="threePointAttempts"
          type="number"
          {...register("threePointAttempts", { valueAsNumber: true })}
        />
        {errors.threePointAttempts && (
          <span className="text-sm text-destructive">{errors.threePointAttempts.message}</span>
        )}
      </Field>
    </div>
  );
}