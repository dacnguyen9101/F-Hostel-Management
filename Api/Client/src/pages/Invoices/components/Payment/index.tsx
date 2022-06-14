import { Button } from '@mui/material'
import React from 'react'
import PaidIcon from '@mui/icons-material/Paid'
import { RestCaller } from '../../../../utils/RestCaller'
import { IInvoiceProps } from '../../interfaces/IInvoiceProps'
interface IPaymentProps {
    invoice: IInvoiceProps
}

export const Payment = (props: IPaymentProps) => {
    const getPaymentUrlAsync = async () => {
        await RestCaller.get(
            `Invoices/create-vnpay?invoiceId=${props.invoice.id}`
        )
    }
    return (
        <>
            <Button
                variant="contained"
                endIcon={<PaidIcon sx={{ fontSize: '1.3rem' }} />}
            >
                Pay the bill
            </Button>
        </>
        // <IconButtonCustom
        //     textColor="#fff"
        //     bgrColor="#495057"
        //     sx={{ width: '2.8rem', height: '2.8rem' }}
        //     // onClick={handleOpenUpdate}
        // >
        //     <Edit sx={{ fontSize: '1.3rem' }} />
        //     Send
        // </IconButtonCustom>
    )
}
