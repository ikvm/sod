﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PWMIS.DataMap.Entity;

namespace SODTest
{
    /// <summary>
    /// OQL 多实体类查询 动态条件构造测试 网友  红枫星空  提供
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            SalesOrder model = new SalesOrder();
            //model.iOrderTypeID = "123";

            //string orderTypeID = model.iOrderTypeID;
            BCustomer bCustomer = new BCustomer();

            OQLCompareFunc<BCustomer,SalesOrder> cmpFun = (cmp,C,S) =>
            {
                OQLCompare cmpResult = null;
                cmpResult = cmp.Comparer(S.iBillID, OQLCompare.CompareType.Equal, 1);
                
                if (!string.IsNullOrEmpty(S.iOrderTypeID))
                    cmpResult = cmpResult & cmp.Comparer(S.iOrderTypeID, OQLCompare.CompareType.Equal, S.iOrderTypeID);

                int iCityID = 30;
                //由于调用了关联实体类的 S.iOrderTypeID 用于条件比较，所以下面需要调用 cmp.NewCompare()
                //cmpResult = cmpResult & cmp.NewCompare().Comparer<int>(C.iCityID, OQLCompare.CompareType.Equal, iCityID);
                //感谢网友 红枫星空 发现此问题

                //或者继续采用下面的写法，但是必须确保 Comparer 方法第一个参数调用为实体类属性，而不是待比较的值
                //且第一个参数的值不能等于第三个参数的值，否则需要调用NewCompare() 方法
                cmpResult = cmpResult & cmp.Comparer(C.iCityID, OQLCompare.CompareType.Equal, iCityID);
                return cmpResult;
            };
           

            OQL oQL = OQL.From(model)
                    .LeftJoin(bCustomer).On(model.iCustomerID, bCustomer.ISID)
                    .Select()
                    .Where(cmpFun)
                    .OrderBy(model.iBillID, "desc")
                .END;

            Console.WriteLine(oQL);
            Console.WriteLine(oQL.PrintParameterInfo());
            Console.ReadLine();
        }
    }
}