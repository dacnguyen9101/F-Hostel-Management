import React, { FC } from 'react'
import FormDialog from '../../../../../components/DialogCustom/FormDialog'
import { useAppDispatch, useAppSelector } from '../../../../../hooks/reduxHook'
import { useForm } from '../../../../../hooks/useForm'
import { IRoom, IRoomValues } from '../../../../../interface/IRoom'
import { updateRoom } from '../../../../../services/RoomService'
import { fetchRoomList } from '../../../../../slices/roomSlice'
import { getItem } from '../../../../../utils/LocalStorageUtils'
import RoomForm from '../../RoomForm'

interface IUpdateRoomDialogProps {
    openDialog: boolean
    handleCloseDialog: () => void
    room: IRoom
}

const UpdateRoomDialog: FC<IUpdateRoomDialogProps> = ({
    openDialog,
    handleCloseDialog,
    room,
}) => {
    const dispatch = useAppDispatch()
    const page = useAppSelector(({ table }) => table.page)
    const pageSize = useAppSelector(({ table }) => table.pageSize)

    const initialValues: IRoomValues = {
        roomName: room.roomName || '',
        maximumPeople: room.maximumPeople || 0,
        numOfWindows: room.numOfWindows || 0,
        numOfDoors: room.numOfDoors || 0,
        numOfBathRooms: room.numOfBathRooms || 0,
        numOfWCs: room.numOfWCs || 0,
        numOfBedRooms: room.numOfBedRooms || 0,
        area: room.area || 0,
        length: room.length || 0,
        width: room.width || 0,
        height: room.height || 0,
        hostelId: room.hostelId || '',
    }
    const { values, handleInputChange } = useForm<IRoomValues>(initialValues)

    const handleUpdateRoom = async () => {
        const hostelId = getItem('currentHostelId')
        const response = await updateRoom(room.id, {
            ...values,
            area: values?.length * values?.width,
            hostelId,
        })
        if (!response.isError) {
            dispatch(fetchRoomList({ hostelId, pageSize, page }))
            handleCloseDialog()
        }
    }
    return (
        <FormDialog
            title="Update Room"
            action="Update"
            openDialog={openDialog}
            handleCloseDialog={handleCloseDialog}
            handleSubmit={handleUpdateRoom}
            maxWidth="md"
        >
            <RoomForm values={values} handleInputChange={handleInputChange} />
        </FormDialog>
    )
}

export default UpdateRoomDialog
