using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.Wpf.Copy
{
    class OrderInfo
    {
        public string TradeID { get; set; }
        public string Time_stamp { get; set; }
        public string Item { get; set; }        
        public string Price { get; set; }
        public string Num { get; set; }
        public string Trouble { get; set; }
        public string Contact { get; set; }
        public string Trade_status { get; set; }
        public string Order_price { get; set; }
        public string Remark { get; set; }
        public string Message { get; set; }        
    }
    /*
        class OrderInfoList
        {
            //public System.Xml.XmlNodeList AccountMsg { get; set; }

            public Clist ValueList = new Clist();

            public bool ReadOrderInfoListFinish = false;
        }

        // 数据结构类

        public class DataNode
        {
            public DataNode()
            {
                Item = "";
                Price = "";
                Num = "";
                Trouble = "";
                Contact = "";
                Trade_status = "";
                Order_price = "";
                Remark = "";
                Message = "";
            }

            public DataNode(string ItemValue, string PriceValue, string NumValue, string TroubleValue,
                            string ContactValue, string Trade_statusValue, string Order_priceValue,
                            string RemarkValue, string MessageValue)
            {
                Item = ItemValue;
                Price = PriceValue;
                Num = NumValue;
                Trouble = TroubleValue;
                Contact = ContactValue;
                Trade_status = Trade_statusValue;
                Order_price = Order_priceValue;
                Remark = RemarkValue;
                Message = MessageValue;
            }

            public string Item;
            public string Price;
            public string Num;
            public string Trouble;
            public string Contact;
            public string Trade_status;
            public string Order_price;
            public string Remark;
            public string Message;
        }

        //  结点类
        public class ListNode
        {
            public ListNode(DataNode NewValue)
            {
                Value = NewValue;
            }

            //前一个
            public ListNode Previous;

            // 后一个
            public ListNode Next;

            // 值
            public DataNode Value;
        }
        // 定义结点之后，开始类线性表的操作编程了.在LIST 类中,采用了，Head ,Tail,  Current，三个指针，使用Append ，
        //MoveFrist,MovePrevious，MoveNext,MoveLast ,Delete,InsertAscending,InsertUnAscending ,Clear 实现移动，添加，
        //删除,升序插入，降序插入，清空链表操作，GetCurrentValue（） 方法取得当前的值。
        public class Clist
        {
            public Clist()
            {
                //构造函数
                //初始化
                ListCountValue = 0;
                Head = null;
                Tail = null;
            }


            // 头指针
            private ListNode Head;

            // 尾指针  
            private ListNode Tail;

            // 当前指针
            private ListNode Current;

            //链表数据的个数
            private int ListCountValue;

            //尾部添加数据 
            public void Append(DataNode DataValue)
            {
                ListNode NewNode = new ListNode(DataValue);

                if (IsNull())
                //如果头指针为空
                {
                    Head = NewNode;
                    Tail = NewNode;
                }
                else
                {
                    Tail.Next = NewNode;
                    NewNode.Previous = Tail;
                    Tail = NewNode;
                }
                Current = NewNode;
                //链表数据个数加一
                ListCountValue += 1;
            }

            //删除当前的数据
            public void Delete()
            {
                //若为空链表
                if (!IsNull())
                {
                    //若删除头
                    if (IsBof())
                    {
                        Head = Current.Next;
                        Current = Head;
                        ListCountValue -= 1;
                        return;
                    }

                    //若删除尾
                    if (IsEof())
                    {
                        Tail = Current.Previous;
                        Current = Tail;
                        ListCountValue -= 1;
                        return;
                    }

                    //若删除中间数据
                    Current.Previous.Next = Current.Next;
                    Current = Current.Previous;
                    Current.Next.Previous = Current;
                    ListCountValue -= 1;
                    return;
                }
            }

            // 向后移动一个数据
            public void MoveNext()
            {
                if (!IsEof()) Current = Current.Next;
            }

            // 向前移动一个数据
            public void MovePrevious()
            {
                if (!IsBof()) Current = Current.Previous;
            }

            // 移动到第一个数据  
            public void MoveFrist()
            {
                Current = Head;
            }


            // 移动到最后一个数据
            public void MoveLast()
            {
                Current = Tail;
            }

            // 判断是否为空链表
            public bool IsNull()
            {
                if (ListCountValue == 0)
                    return true;

                return false;
            }

            // 判断是否为到达尾  
            public bool IsEof()
            {
                if (Current == Tail)
                    return true;
                return false;
            }

            // 判断是否为到达头部
            public bool IsBof()
            {
                if (Current == Head)
                    return true;
                return false;

            }

            public DataNode GetCurrentValue()
            {
                if (null == Current)
                    return null;
                return Current.Value;
            }

            // 取得链表的数据个数
            public int ListCount
            {
                get
                {
                    return ListCountValue;
                }
            }


            // 清空链表
            public void Clear()
            {
                MoveFrist();
                while (!IsNull())
                {
                    //若不为空链表,从尾部删除  
                    Delete();
                }
            }


            // 在当前位置前插入数据
            public void Insert(DataNode DataValue)
            {
                ListNode NewNode = new ListNode(DataValue);
                if (IsNull())
                {
                    //为空表，则添加
                    Append(DataValue);
                    return;
                }

                if (IsBof())
                {
                    //为头部插入
                    NewNode.Next = Head;
                    Head.Previous = NewNode;
                    Head = NewNode;
                    Current = Head;
                    ListCountValue += 1;
                    return;
                }
                //中间插入
                NewNode.Next = Current;
                NewNode.Previous = Current.Previous;
                Current.Previous.Next = NewNode;
                Current.Previous = NewNode;
                Current = NewNode;
                ListCountValue += 1;
            }

            // 进行升序插入  
            public void InsertAscending(DataNode InsertValue)
            {
                //参数：InsertValue 插入的数据
                //为空链表
                if (IsNull())
                {
                    //添加
                    Append(InsertValue);
                    return;
                }

                //移动到头
                MoveFrist();
                if (InsertValue.AccountName.CompareTo(GetCurrentValue().AccountName) <= 0)
                {
                    //满足条件，则插入，退出
                    Insert(InsertValue);
                    return;
                }
                while (true)
                {
                    if (InsertValue.AccountName.CompareTo(GetCurrentValue().AccountName) <= 0)
                    {
                        //满族条件，则插入，退出
                        Insert(InsertValue);
                        break;
                    }
                    if (IsEof())
                    {
                        //尾部添加
                        Append(InsertValue);
                        break;
                    }
                    //移动到下一个指针
                    MoveNext();
                }
            }
            // 进行降序插入
            public void InsertUnAscending(DataNode InsertValue)
            {
                //参数：InsertValue 插入的数据                      
                //为空链表
                if (IsNull())
                {
                    //添加
                    Append(InsertValue);
                    return;
                }

                //移动到头
                MoveFrist();
                if (InsertValue.AccountName.CompareTo(GetCurrentValue().AccountName) > 0)
                {
                    //满足条件，则插入，退出
                    Insert(InsertValue);
                    return;
                }
                while (true)
                {
                    if (InsertValue.AccountName.CompareTo(GetCurrentValue().AccountName) > 0)
                    {
                        //满族条件，则插入，退出
                        Insert(InsertValue);
                        break;
                    }
                    if (IsEof())
                    {
                        //尾部添加
                        Append(InsertValue);
                        break;
                    }

                    //移动到下一个指针
                    MoveNext();
                }
            }
 
        }
 */
}
