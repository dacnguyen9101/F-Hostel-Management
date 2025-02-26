import React, { ChangeEvent, FC, useEffect } from 'react'
import * as Styled from './styles'
import ComboBox from '../../../../../components/ComboBox'
import InputField from '../../../../../components/Input/InputField'
import { IField } from '../../../../../interface/IField'
import { InputAdornment } from '@mui/material'
import { getItem } from '../../../../../utils/LocalStorageUtils'
import { IRoom } from '../../../../../interface/IRoom'
import { IHostel } from '../../../../../interface/IHostel'
import { ICommitmentValues } from '../../../../../interface/ICommitment'
interface IStep1Props {
    values: ICommitmentValues
    setValues: (values: ICommitmentValues) => void
    handleInputChange: (event: ChangeEvent<HTMLInputElement>) => void

    roomInfo: IRoom
    setRoomInfo: (roomInfo: IRoom) => void
    roomOptions: IRoom[]

    hostelInfo: IHostel
    setHostelInfo: (hostelInfo: IHostel) => void
    hostelOptions: IHostel[]

    isUpdate: boolean
}

const fields: IField[] = [
    {
        label: 'Start Date',
        name: 'startDate',
        type: 'date',
        required: true,
    },
    {
        label: 'End Date',
        name: 'endDate',
        type: 'date',
        required: true,
    },
    {
        label: 'Payment Day',
        name: 'paymentDate',
        type: 'number',
        required: false,
        endAdornment: <InputAdornment position="end">days</InputAdornment>,
        inputProps: { min: 1, max: 31 },
    },
    {
        label: 'Allowed of days overdue',
        name: 'dateOverdue',
        type: 'number',
        required: true,
        endAdornment: <InputAdornment position="end">days</InputAdornment>,
        inputProps: { min: 0, max: 31 },
    },
    {
        label: 'Price',
        name: 'price',
        type: 'number',
        required: true,
        endAdornment: <InputAdornment position="end">vnd</InputAdornment>,
    },
    {
        label: 'Compensation Fee',
        name: 'compensation',
        type: 'number',
        required: true,
        endAdornment: <InputAdornment position="end">vnd</InputAdornment>,
    },
]

const Step1: FC<IStep1Props> = ({
    values,
    setValues,
    handleInputChange,
    roomInfo,
    setRoomInfo,
    roomOptions,
    hostelInfo,
    setHostelInfo,
    hostelOptions,
    isUpdate,
}) => {
    useEffect(() => {
        setValues({ ...values, roomId: roomInfo?.id || '' })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [roomInfo])
    const currentHostelId = getItem('currentHostelId')
    return (
        <Styled.ContainerStep>
            <Styled.LeftSide>
                {fields.slice(0, 4).map((field) => (
                    <InputField
                        key={field.name}
                        label={field.label}
                        name={field.name}
                        value={values[field.name]}
                        onChange={handleInputChange}
                        type={field.type}
                        required={field.required}
                        disabled={field.disabled}
                        endAdornment={field.endAdornment}
                        inputProps={field.inputProps}
                    />
                ))}
            </Styled.LeftSide>
            <Styled.RightSide>
                {!currentHostelId && (
                    <ComboBox
                        label="Hostel"
                        options={hostelOptions}
                        optionLabel="name"
                        valueAutocomplete={hostelInfo}
                        setValueAutocomplete={setHostelInfo}
                        defaultValue={hostelOptions?.[0]}
                    />
                )}

                <ComboBox
                    label="Room"
                    options={roomOptions}
                    optionLabel="roomName"
                    valueAutocomplete={roomInfo}
                    setValueAutocomplete={setRoomInfo}
                    disabled={
                        !hostelInfo ||
                        !Object.keys(hostelInfo).length ||
                        isUpdate
                    }
                    defaultValue={roomOptions?.[0]}
                />
                {fields.slice(4, 8).map((field) => (
                    <InputField
                        key={field.name}
                        label={field.label}
                        name={field.name}
                        value={values[field.name]}
                        onChange={handleInputChange}
                        type={field.type}
                        required={field.required}
                        disabled={field.disabled}
                        endAdornment={field.endAdornment}
                        inputProps={field.inputProps}
                    />
                ))}
            </Styled.RightSide>
        </Styled.ContainerStep>
    )
}

export default Step1
