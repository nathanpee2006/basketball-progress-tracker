import { z } from "zod";

const drillSchema = z.object({
  name: z.string().min(1, "Drill name is required"),
  completionTimeInSeconds: z.number().positive("Must be greater than 0"),
});

export const sessionFormSchema = z
  .object({
    date: z.string().min(1, "Date is required"),

    paintMakes: z.number().min(0),
    paintAttempts: z.number().min(0),

    midrangeMakes: z.number().min(0),
    midrangeAttempts: z.number().min(0),

    threePointMakes: z.number().min(0),
    threePointAttempts: z.number().min(0),

    freeThrowMakes: z.number().min(0),
    freeThrowAttempts: z.number().min(0),

    drills: z.array(drillSchema),
  })
  .refine((data) => data.paintMakes <= data.paintAttempts, {
    message: "Makes cannot exceed attempts",
    path: ["paintMakes"],
  })
  .refine((data) => data.midrangeMakes <= data.midrangeAttempts, {
    message: "Makes cannot exceed attempts",
    path: ["midrangeMakes"],
  })
  .refine((data) => data.threePointMakes <= data.threePointAttempts, {
    message: "Makes cannot exceed attempts",
    path: ["threePointMakes"],
  })
  .refine((data) => data.freeThrowMakes <= data.freeThrowAttempts, {
    message: "Makes cannot exceed attempts",
    path: ["freeThrowMakes"],
  });

export type SessionFormValues = z.infer<typeof sessionFormSchema>;
