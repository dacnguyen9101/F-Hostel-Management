import * as React from 'react'

import * as Styled from './styles'

interface ICustomToolbarProps {
    children?: any
    title: string
}

const CustomToolbar: React.FunctionComponent<ICustomToolbarProps> = ({
    children,
    title = 'Table Title',
}) => {
    return (
        <Styled.GridToolbar>
            <Styled.WrapperLeft>
                <Styled.Title variant="subtitle2">
                    <strong>{title}</strong>
                </Styled.Title>
                {/* <SearchInput loading={true} width="250px" height="34px" /> */}
            </Styled.WrapperLeft>
            <Styled.WrapperRight>{children}</Styled.WrapperRight>
        </Styled.GridToolbar>
    )
}

export default CustomToolbar
