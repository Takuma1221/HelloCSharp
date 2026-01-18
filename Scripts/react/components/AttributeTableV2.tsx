import React, { useMemo } from 'react';
import {
    useReactTable,
    getCoreRowModel,
    getSortedRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    flexRender,
    createColumnHelper,
    type SortingState,
    type ColumnFiltersState,
} from '@tanstack/react-table';
import type { AttributeDefinition } from '../shared/types';

interface Props {
    attributes: AttributeDefinition[];
    onEdit: (attr: AttributeDefinition) => void;
    onDelete: (id: number) => void;
}

const columnHelper = createColumnHelper<AttributeDefinition>();

export const AttributeTableV2: React.FC<Props> = ({ attributes, onEdit, onDelete }) => {
    const [sorting, setSorting] = React.useState<SortingState>([]);
    const [columnFilters, setColumnFilters] = React.useState<ColumnFiltersState>([]);
    const [globalFilter, setGlobalFilter] = React.useState('');

    // カラム定義
    const columns = useMemo(
        () => [
            columnHelper.accessor('id', {
                header: 'ID',
                size: 80,
                enableSorting: true,
            }),
            columnHelper.accessor('attributeName', {
                header: '属性名',
                size: 200,
                enableSorting: true,
                enableColumnFilter: true,
            }),
            columnHelper.accessor('dataType', {
                header: 'データ型',
                size: 120,
                enableSorting: true,
                cell: (info) => {
                    const typeMap: Record<string, string> = {
                        String: '文字列',
                        Integer: '整数',
                        Decimal: '小数',
                        Boolean: '真偽値',
                        Date: '日付',
                    };
                    return typeMap[info.getValue()] || info.getValue();
                },
            }),
            columnHelper.accessor('isRequired', {
                header: '必須',
                size: 80,
                enableSorting: true,
                cell: (info) => (
                    <span className={`badge ${info.getValue() ? 'bg-danger' : 'bg-secondary'}`}>
                        {info.getValue() ? '必須' : '任意'}
                    </span>
                ),
            }),
            columnHelper.accessor('defaultValue', {
                header: 'デフォルト値',
                size: 150,
                cell: (info) => info.getValue() || '-',
            }),
            columnHelper.display({
                id: 'actions',
                header: '操作',
                size: 150,
                cell: (info) => (
                    <div className="btn-group btn-group-sm">
                        <button
                            className="btn btn-outline-primary"
                            onClick={() => onEdit(info.row.original)}
                        >
                            ✏️ 編集
                        </button>
                        <button
                            className="btn btn-outline-danger"
                            onClick={() => onDelete(info.row.original.id)}
                        >
                            🗑️ 削除
                        </button>
                    </div>
                ),
            }),
        ],
        [onEdit, onDelete]
    );

    // テーブルインスタンス作成
    const table = useReactTable({
        data: attributes,
        columns,
        state: {
            sorting,
            columnFilters,
            globalFilter,
        },
        onSortingChange: setSorting,
        onColumnFiltersChange: setColumnFilters,
        onGlobalFilterChange: setGlobalFilter,
        getCoreRowModel: getCoreRowModel(),
        getSortedRowModel: getSortedRowModel(),
        getFilteredRowModel: getFilteredRowModel(),
        getPaginationRowModel: getPaginationRowModel(),
        initialState: {
            pagination: {
                pageSize: 10,
            },
        },
    });

    return (
        <div>
            {/* 検索フィルター */}
            <div className="mb-3">
                <input
                    type="text"
                    className="form-control"
                    placeholder="🔍 全体検索..."
                    value={globalFilter ?? ''}
                    onChange={(e) => setGlobalFilter(e.target.value)}
                />
            </div>

            {/* テーブル */}
            <div className="table-responsive">
                <table className="table table-hover table-bordered">
                    <thead className="table-light">
                        {table.getHeaderGroups().map((headerGroup) => (
                            <tr key={headerGroup.id}>
                                {headerGroup.headers.map((header) => (
                                    <th
                                        key={header.id}
                                        style={{ width: header.getSize() }}
                                        className={
                                            header.column.getCanSort()
                                                ? 'cursor-pointer user-select-none'
                                                : ''
                                        }
                                        onClick={header.column.getToggleSortingHandler()}
                                    >
                                        <div className="d-flex align-items-center justify-content-between">
                                            {flexRender(
                                                header.column.columnDef.header,
                                                header.getContext()
                                            )}
                                            {header.column.getCanSort() && (
                                                <span>
                                                    {{
                                                        asc: ' 🔼',
                                                        desc: ' 🔽',
                                                    }[header.column.getIsSorted() as string] ?? ' ↕️'}
                                                </span>
                                            )}
                                        </div>
                                    </th>
                                ))}
                            </tr>
                        ))}
                    </thead>
                    <tbody>
                        {table.getRowModel().rows.length === 0 ? (
                            <tr>
                                <td colSpan={columns.length} className="text-center text-muted">
                                    データがありません
                                </td>
                            </tr>
                        ) : (
                            table.getRowModel().rows.map((row) => (
                                <tr key={row.id}>
                                    {row.getVisibleCells().map((cell) => (
                                        <td key={cell.id}>
                                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                        </td>
                                    ))}
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* ページネーション */}
            <div className="d-flex justify-content-between align-items-center mt-3">
                <div>
                    <span className="text-muted">
                        {table.getState().pagination.pageIndex + 1} / {table.getPageCount()} ページ
                        （全 {table.getFilteredRowModel().rows.length} 件）
                    </span>
                </div>
                <div className="btn-group">
                    <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => table.setPageIndex(0)}
                        disabled={!table.getCanPreviousPage()}
                    >
                        {'<<'}
                    </button>
                    <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => table.previousPage()}
                        disabled={!table.getCanPreviousPage()}
                    >
                        {'<'}
                    </button>
                    <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => table.nextPage()}
                        disabled={!table.getCanNextPage()}
                    >
                        {'>'}
                    </button>
                    <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => table.setPageIndex(table.getPageCount() - 1)}
                        disabled={!table.getCanNextPage()}
                    >
                        {'>>'}
                    </button>
                </div>
            </div>
        </div>
    );
};
