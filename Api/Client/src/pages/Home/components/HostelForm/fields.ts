import { IField } from '../../../../interface/IField'

export const fields: IField[] = [
    {
        label: 'Hostel Name',
        name: 'name',
        type: 'text',
        required: true,
    },
    {
        label: 'Address',
        name: 'address',
        type: 'text',
        required: true,
    },
    {
        label: 'Time span of QR code (minutes)',
        name: 'timeSpan',
        type: 'number',
        required: true,
    },
]
